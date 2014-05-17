using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Drone.Lib.Compilers;
using Drone.Lib.Configs;
using Drone.Lib.Helpers;
using NLog;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Text;

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

		private IEnumerable<string> GetBaseReferenceFiles()
		{
			var appPath = this.GetDroneAppPath();
			
			yield return Path.Combine(appPath, "Drone.Lib.dll");
			yield return Path.Combine(appPath, "Nlog.dll");
			yield return Path.Combine(appPath, "Newtonsoft.Json.dll");
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

				var resolvedBaseReferences = this.GetBaseReferenceFiles();
				var resolvedConfigReferences = this.ResolveReferenceFiles(config.ReferenceFiles, config.Properties);

				var referenceFiles = resolvedBaseReferences.Concat(resolvedConfigReferences).Distinct();

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

		private IEnumerable<string> ResolveReferenceFiles(IEnumerable<string> files, JObject properties)
		{
			var sb = new StringBuilder();

			foreach(var file in files)
			{
				sb.Clear();

				if(Regex.IsMatch(file, @"\%.+\%"))
				{
					sb.Append(Environment.ExpandEnvironmentVariables(file));
				}

				var matches = Regex.Matches(file, @"\{.+\}");

				foreach(var match in matches.OfType<Match>())
				{
					if(!match.Success)
						continue;

					var key = match.Value.Substring(1, match.Value.Length - 1);
					var token = null as JToken;

					if(properties.TryGetValue(key, out token))
					{
						sb.Replace(match.Value, token.ToString());
					}
					else
					{
						this.Log.Warn("unable to find property: {0} in config to set replace in path");
					}
				}

				yield return sb.ToString();
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