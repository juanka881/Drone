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

		public IEnumerable<string> GetBaseReferenceFiles()
		{
			var appPath = this.GetAppPathBaseDir();

			yield return Path.Combine(appPath, "Drone.Lib.dll");
			yield return Path.Combine(appPath, "Nlog.dll");
			yield return Path.Combine(appPath, "Newtonsoft.Json.dll");
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

			foreach(var source in config.SourceFiles)
				cache.Add(new FileInfo(source));

			foreach(var reference in config.ReferenceFiles)
				cache.Add(new FileInfo(reference));

			this.store.Save(this.GetCacheFileName(config), cache);
		}

		private string GetAppPathBaseDir()
		{
			var codeBase = System.Reflection.Assembly.GetEntryAssembly().CodeBase;
			var uri = new UriBuilder(codeBase);
			var path = Uri.UnescapeDataString(uri.Path);
			return Path.GetDirectoryName(path);
		}

		private CSharpCompilerResult CompileCore(DroneConfig config)
		{
			try
			{
				if (config == null)
					throw new ArgumentNullException("config");

				var compiler = new CSharpCompiler();

				this.log.Debug("checking if output dir exists '{0}'", config.BuildDirPath);

				if (!Directory.Exists(config.BuildDirPath))
				{
					this.log.Debug("output dir not found. creating output bin dir");
					Directory.CreateDirectory(config.BuildDirPath);
				}

				var configDirpath = Path.GetDirectoryName(config.FilePath);

				var resolvedBaseReferences = this.GetBaseReferenceFiles();
				var resolvedConfigReferences = this.ResolveReferenceFiles(config.ReferenceFiles, configDirpath);

				var referenceFiles = resolvedBaseReferences.Concat(resolvedConfigReferences).Distinct().ToList();

				var sourceFiles = this.ResolveSourceFiles(config.SourceFiles, configDirpath).ToList();

				this.log.Debug("creating csharp compiler args");

				var args = new CSharpCompilerArgs(
					config.DirPath,
					config.AssemblyFilePath,
					sourceFiles,
					referenceFiles);

				if (DroneContext.Flags != null && DroneContext.Flags.IsDebugEnabled)
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

				this.log.Debug("calling csc compiler '{0}'...", config.FileName);

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

		public void Compile(DroneConfig config, LogLevel logLevel)
		{
			if (this.IsRecompileNeeded(config))
			{
				var result = this.CompileCore(config);
				
				try
				{
					if (result.IsSuccess)
					{
						this.CreateCache(config);
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