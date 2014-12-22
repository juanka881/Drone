using Drone.Lib.Helpers;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Drone.Lib.Core
{
	public class DroneService
	{
		private readonly JsonStore store;
		private readonly DroneCompiler compiler;
		private readonly DroneLoader loader;
		private readonly DroneTaskRunner taskRunner;
		private readonly Logger log;

		public DroneService()
		{
			this.log = DroneLogManager.GetLog();
			this.store = new JsonStore();
			this.compiler = new DroneCompiler();
			this.loader = new DroneLoader();
			this.taskRunner = new DroneTaskRunner(new DroneTaskHandlerFactory());
		}

		private DroneModule CompileAndLoadModule(DroneEnv env, LogLevel logLevel)
		{
			this.compiler.Compile(env, logLevel);
			var module = this.loader.Load(env.Config);
			return module;
		}

		private IList<string> GetFixedFilePaths(string basePath, IEnumerable<string> files)
		{
			var relPaths = from filePath in files
						   let absPath = Path.IsPathRooted(filePath) ? filePath : Path.Combine(basePath, filePath)
						   select absPath;

			return relPaths.ToList();
		}

		private bool EnsureFilesExists(string basePath, IEnumerable<string> files)
		{
			var notFound = false;

			foreach (var file in files)
			{
				var filename = file;

				if(!Path.IsPathRooted(filename))
					filename = Path.Combine(basePath, file);

					if (!File.Exists(filename))
					{
						notFound = true;
						this.log.Error("file not found: {0}", file);
					}
			}

			return notFound;
		}

		public string GetAppPathBaseDir()
		{
			var codeBase = System.Reflection.Assembly.GetEntryAssembly().CodeBase;
			var uri = new UriBuilder(codeBase);
			var path = Uri.UnescapeDataString(uri.Path);
			return Path.GetDirectoryName(path);
		}

		public DroneConfig LoadConfig(string configFilePath)
		{
			if(string.IsNullOrWhiteSpace(configFilePath))
				throw new ArgumentException("configFilePath is empty or null", "configFilePath");

			using (var fs = File.OpenRead(configFilePath))
			{
				var config = this.store.Load<DroneConfig>(fs);
				config.SetConfigFilename(configFilePath);
				return config;
			}
		}

		public void SaveConfig(DroneConfig config)
		{
			if(config == null)
				throw new ArgumentNullException("config");

			this.store.Save(config.FilePath, config);
		}

		public void AddFiles(DroneConfig config, IList<string> sourceFiles, IList<string> referenceFiles)
		{
			if(config == null)
				throw new ArgumentNullException("config");

			if(sourceFiles == null)
				throw new ArgumentNullException("sourceFiles");

			if(referenceFiles == null)
				throw new ArgumentNullException("referenceFiles");

			var sourcesToAdd = sourceFiles.Except(config.SourceFiles).ToList();

			var referencesToAdd = referenceFiles.Except(config.ReferenceFiles).ToList();

			if (sourcesToAdd.Count == 0 && referencesToAdd.Count == 0)
			{
				this.log.Warn("no files added. files already exists in config");
				return;
			}

			var fileNotFound = this.EnsureFilesExists(config.DirPath, sourcesToAdd.Concat(referencesToAdd));

			if (fileNotFound)
				return;

			config.SourceFiles.AddRange(sourcesToAdd);
			config.ReferenceFiles.AddRange(referencesToAdd);

			foreach (var file in sourcesToAdd.Concat(referencesToAdd))
				this.log.Info("added '{0}'", file);
		}

		public void RemoveFiles(DroneConfig config, IList<string> sourceFiles, IList<string> referenceFiles)
		{
			if(config == null)
				throw new ArgumentNullException("config");

			if(sourceFiles == null)
				throw new ArgumentNullException("sourceFiles");

			if(referenceFiles == null)
				throw new ArgumentNullException("referenceFiles");

			var sourcesRemoved = sourceFiles
				.Where(x => config.SourceFiles.Remove(x))
				.ToList();

			var referencesRemoved = referenceFiles
				.Where(x => config.ReferenceFiles.Remove(x))
				.ToList();

			if (sourcesRemoved.Count == 0 && referencesRemoved.Count == 0)
			{
				this.log.Warn("nothing to remove. files do not exist in config");
				return;
			}

			foreach (var file in sourcesRemoved.Concat(referencesRemoved))
				this.log.Info("removed '{0}'", file);
		}

		public void RunTasks(DroneEnv env, IEnumerable<string> taskNames)
		{
			DroneEnv.Set(env);
			var module = this.CompileAndLoadModule(env, LogLevel.Debug);
			this.taskRunner.Run(module, taskNames, env);
		}

		public IEnumerable<DroneTask> GetTasks(DroneEnv env)
		{
			DroneEnv.Set(env);
			var module = this.CompileAndLoadModule(env, LogLevel.Debug);
				
			foreach (var task in module.Tasks)
			{
				var taskCopy = new DroneTask(task.Name, task.Dependencies);
				yield return taskCopy;
			}
		}

		public void CompileTasks(DroneEnv env, LogLevel logLevel)
		{
			DroneEnv.Set(env);
			this.compiler.Compile(env, logLevel);
		}

		public void InitDroneDir(DroneConfig config)
		{
			if(config == null)
				throw new ArgumentNullException("config");

			if(Directory.Exists(config.DroneDirPath))
				return;

			Directory.CreateDirectory(config.DroneDirPath);

			if(!Directory.Exists(config.DroneSourceDirPath))
				Directory.CreateDirectory(config.DroneSourceDirPath);

			if(!Directory.Exists(config.DroneReferencesDirPath))
				Directory.CreateDirectory(config.DroneReferencesDirPath);

			var referenceFiles = this.compiler.GetBaseReferenceFiles(this.GetAppPathBaseDir());

			foreach(var referenceFile in referenceFiles)
			{
				var destFile = Path.Combine(config.DroneReferencesDirPath, Path.GetFileName(referenceFile));
				File.Copy(referenceFile, destFile);
			}
		}
	}
}