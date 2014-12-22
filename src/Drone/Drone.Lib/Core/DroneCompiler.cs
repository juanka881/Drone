using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Drone.Lib.Compilers;
using Drone.Lib.Helpers;
using NLog;
using System.Text.RegularExpressions;
using System.Text;
using Drone.Lib.FileSystem;

namespace Drone.Lib.Core
{
	public class DroneCompiler
	{
		public static readonly string CacheFileName = "cache.json";

		public readonly Logger log;

		private readonly JsonStore store;

		public DroneCompiler()
		{
			this.log = DroneLogManager.GetLog();
			this.store = new JsonStore();
		}

		public bool IsRecompileNeeded(DroneConfig config)
		{
			this.EnsureBuildDirExits(config);

			if(!File.Exists(config.AssemblyFilePath))
				return true;

			if(!File.Exists(this.GetCacheFileName(config)))
				return true;

			var cache = this.store.Load<FileMetadataCache>(this.GetCacheFileName(config));
			return cache.HasChanges();
		}

		public IEnumerable<string> GetBaseReferenceFiles(string basePath)
		{
			yield return Path.Combine(basePath, "Drone.Lib.dll");
			yield return Path.Combine(basePath, "Nlog.dll");
			yield return Path.Combine(basePath, "Newtonsoft.Json.dll");
		}

		private void EnsureBuildDirExits(DroneConfig config)
		{
			if(config == null)
				throw new ArgumentNullException("config");

			if(!Directory.Exists(config.BuildDirPath))
				Directory.CreateDirectory(config.BuildDirPath);
		}

		private string GetCacheFileName(DroneConfig config)
		{
			return Path.Combine(config.BuildDirPath, CacheFileName);
		}

		private void CreateCache(DroneConfig config)
		{
			var cache = new FileMetadataCache();

			cache.Add(new FileInfo(config.FilePath));

			foreach(var source in this.ResolveSourceFiles(config.SourceFiles, config.DirPath))
				cache.Add(new FileInfo(source));

			foreach(var reference in this.ResolveReferenceFiles(config.ReferenceFiles, config.DirPath))
				cache.Add(new FileInfo(reference));

			this.store.Save(this.GetCacheFileName(config), cache);
		}

		private CSharpCompilerResult CompileCore(DroneEnv env)
		{
			try
			{
				if (env == null)
					throw new ArgumentNullException("env");

				var compiler = new CSharpCompiler();

				this.log.Debug("checking if output dir exists '{0}'", env.Config.BuildDirPath);

				if (!Directory.Exists(env.Config.BuildDirPath))
				{
					this.log.Debug("output dir not found. creating output bin dir");
					Directory.CreateDirectory(env.Config.BuildDirPath);
				}

				var configDirpath = Path.GetDirectoryName(env.Config.FilePath);

				var resolvedBaseReferences = this.GetBaseReferenceFiles(env.Config.DroneReferencesDirPath);
				var resolvedConfigReferences = this.ResolveReferenceFiles(env.Config.ReferenceFiles, configDirpath);

				var referenceFiles = resolvedBaseReferences.Concat(resolvedConfigReferences).Distinct().ToList();

				var sourceFiles = this.ResolveSourceFiles(env.Config.SourceFiles, configDirpath).ToList();

				this.log.Debug("creating csharp compiler args");

				var args = new CSharpCompilerArgs(
					env.Config.DirPath,
					env.Config.AssemblyFilePath,
					sourceFiles,
					referenceFiles);

				if (env.Flags.IsDebugEnabled)
				{
					args.Debug = true;
					args.Optimize = false;
				}
				else
				{
					args.Debug = false;
					args.Optimize = true;
				}

				if (this.log.IsDebugEnabled)
				{
					this.log.Debug("csharp args");
					this.log.Debug("work dir: '{0}'", args.WorkDir);
					this.log.Debug("output filepath: '{0}'", args.OutputFilepath);
					this.log.Debug("source files:");

					foreach (var file in sourceFiles)
						this.log.Debug(file);

					this.log.Debug("reference files:");

					foreach (var file in referenceFiles)
						this.log.Debug(file);
				}

				this.log.Debug("calling csc compiler '{0}'...", env.Config.FileName);

				var result = compiler.Compile(args);

				this.log.Debug("csc result: {0}", result.IsSuccess);
				this.log.Debug("exit code: {0}", result.ExiteCode);

				if (result.IsSuccess)
				{
					this.log.Debug("output assembly filepath: '{0}'", result.Success.OutputAssemblyFilepath);
				}

				return result;
			}
			catch (Exception ex)
			{
				return CSharpCompilerResult.GetFailure(-1,
					TimeSpan.Zero,
					ex,
					Enumerable.Empty<string>(),
					Enumerable.Empty<string>(),
					Enumerable.Empty<string>());
			}
		}

		private IEnumerable<string> ResolveSourceFiles(IList<string> files, string configDirPath)
		{
			foreach(var file in files)
			{
				var fullpath = file;

				if(!Path.IsPathRooted(file))
					fullpath = Path.Combine(configDirPath, file);

				yield return fullpath;
			}
		}

		private IEnumerable<string> ResolveReferenceFiles(IEnumerable<string> files, string configDirPath)
		{
			var sb = new StringBuilder();

			foreach(var file in files)
			{
				sb.Clear();

				var fullPath = file;

				if(!Path.IsPathRooted(fullPath))
					fullPath = Path.Combine(configDirPath, file);

				if(Regex.IsMatch(fullPath, @"\%.+\%"))
				{
					sb.Append(Environment.ExpandEnvironmentVariables(fullPath));
				}
				else
				{
					sb.Append(fullPath);
				}

				yield return sb.ToString();
			}
		}

		public void Compile(DroneEnv env, LogLevel logLevel)
		{
			if (this.IsRecompileNeeded(env.Config))
			{
				var result = this.CompileCore(env);
				
				try
				{
					if (result.IsSuccess)
					{
						this.CreateCache(env.Config);
						this.log.Log(logLevel, "compiled ({0})", HumanTime.Format(result.TimeElapsed));
					}
					else
					{
						throw DroneCompilerException.Get(result);
					}
				}
				finally
				{
					this.LogResult(result);
				}
			}
			else
			{
				this.log.Log(logLevel, "compiliation skipped, all files up to date");
			}
		}

		public void LogResult(CSharpCompilerResult result)
		{
			if(result == null)
				throw new ArgumentNullException("result");

			foreach (var line in result.OutputTextLines)
				this.log.Info(line);

			foreach (var line in result.WarningTextLines)
				this.log.Warn(line);

			if (!result.IsSuccess)
			{
				foreach (var line in result.Failure.ErrorTextLines)
					this.log.Error(line);

				if(result.Failure.Exception != null)
					this.log.ExceptionAndData(result.Failure.Exception);
			}
		}
	}
}