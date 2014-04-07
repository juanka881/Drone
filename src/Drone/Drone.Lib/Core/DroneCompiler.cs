using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Drone.Lib.Compilers;
using Drone.Lib.Configs;
using Drone.Lib.Helpers;
using NLog;
using System.Text.RegularExpressions;

namespace Drone.Lib.Core
{
	public class DroneCompiler
	{
		public Logger Log { get; set; }

		private static readonly string CompileStatusFilename = "compile-status.json";
		
		public bool NeedsRecompile(DroneConfig config)
		{
			//if (File.Exists(Path.Combine(config.BinDirpath, CompileStatusFilename)))
			//	return true;

			//if (File.Exists(config.AssemblyFilepath))
			//	return true;

			return true;
		}

		private IList<string> GetDroneReferenceFiles()
		{
			var path = this.GetDroneAppPath();
			var list = new List<string>();

			list.Add(Path.Combine(path, "Drone.Lib.dll"));
			list.Add(Path.Combine(path, "NLog.dll"));

			return list;
		}

		private string GetDroneAppPath()
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

				this.Log.Debug("checking if output dir exists '{0}'", config.BinDirpath);

				if (!Directory.Exists(config.BinDirpath))
				{
					this.Log.Debug("output dir not found. creating output bin dir");
					Directory.CreateDirectory(config.BinDirpath);
				}

				var resolvedReferenceFiles = this.GetResolvedDroneReferenceFiles(config.ReferenceFiles);

				var referenceFiles = resolvedReferenceFiles.Concat(this.GetDroneReferenceFiles());

				this.Log.Debug("creating csharp compiler args");

				var args = new CSharpCompilerArgs(
					config.Dirname,
					config.AssemblyFilepath,
					config.SourceFiles,
					referenceFiles);

				if (this.Log.IsDebugEnabled)
				{
					this.Log.Debug("csharp args");
					this.Log.Debug("work dir: '{0}'", args.WorkDir);
					this.Log.Debug("output filepath: '{0}'", args.OutputFilepath);
					this.Log.Debug("source files:");

					foreach (var file in args.SourceFiles)
						this.Log.Debug(file);

					this.Log.Debug("reference files:");

					foreach (var file in args.ReferenceFiles)
						this.Log.Debug(file);
				}

				this.Log.Debug("calling csc compiler '{0}'...", config.Filename);

				var result = compiler.Compile(args);

				this.Log.Debug("csc result: {0}", result.IsSuccess);
				this.Log.Debug("exit code: {0}", result.ExiteCode);

				if (result.IsSuccess)
				{
					this.Log.Debug("output assembly filepath: '{0}'", result.Success.OutputAssemblyFilepath);

					//this.Log.Debug("deleting 'needs compile' marker file");
					//File.Delete(Path.Combine(config.BinDirpath, CompileStatusFilename));
				}
				else
				{
					//File.WriteAllText(Path.Combine(config.BinDirpath, CompileStatusFilename), string.Empty);
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

		private IEnumerable<string> GetResolvedDroneReferenceFiles(IEnumerable<DroneReferenceFile> files)
		{
			foreach(var file in files)
			{
				var path = file.Path;

				if(file.Type == DroneReferenceFileType.File)
				{
					if(Regex.IsMatch(path, @"\%.+\%"))
					{
						path = Environment.ExpandEnvironmentVariables(path);
					}
				}

				yield return path;
			}
		}

		public void Compile(DroneConfig config)
		{
			if (this.NeedsRecompile(config))
			{
				var result = this.CompileCore(config);

				try
				{
					if (result.IsSuccess)
					{
						this.Log.Info("compiled ({0})", HumanTime.Format(result.TimeElapsed));
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
				this.Log.Info("compiliation skipped, all files up to date");
			}

			this.Log.Info(string.Empty);
		}

		public void LogResult(CSharpCompilerResult result)
		{
			if(result == null)
				throw new ArgumentNullException("result");

			foreach (var line in result.OutputTextLines)
				this.Log.Info(line);

			foreach (var line in result.WarningTextLines)
				this.Log.Warn(line);

			if (!result.IsSuccess)
			{
				foreach (var line in result.Failure.ErrorTextLines)
					this.Log.Error(line);

				if(result.Failure.Exception != null)
					this.Log.ExceptionAndData(result.Failure.Exception);
			}
		}
	}
}