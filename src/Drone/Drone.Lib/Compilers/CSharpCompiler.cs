using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.DotNet;
using Drone.Lib.Helpers;
using System.Diagnostics;
using System.IO;

namespace Drone.Lib.Compilers
{
	public class CSharpCompiler
	{
		private static readonly string ReferenceSwitch = "/r:";
		private static readonly string OutputSwitch = "/out:";
		private static readonly string OptimizeSwitch = "/optimize";
		private static readonly string TargetSwitch = "/target:library";
		private static readonly string NoLogoSwitch = "/nologo";

		private readonly IList<string> outputTextLines;
		private readonly IList<string> warningTextLines;
		private readonly IList<string> errorTextLines;

		public CSharpCompiler()
		{
			this.outputTextLines = new List<string>(10);
			this.warningTextLines = new List<string>(10);
			this.errorTextLines = new List<string>(10);
		}

		public CSharpCompilerResult Compile(CSharpCompilerArgs args)
		{
			if (args == null)
				throw new ArgumentNullException("args");

			var command = string.Empty;
			var commandArgs = string.Empty;
			var sw = new Stopwatch();
			var responseFilename = string.Empty;
			
			try
			{
				sw.Start();

				var framework = DotNetFramework.Version40;
				responseFilename = Path.GetTempFileName();
				commandArgs = string.Format("@{0}", responseFilename);

				command = framework.CSharpCompilerBinFilepath;
				var responseFileText = this.GetCompilerArgString(args);
				File.WriteAllText(responseFilename, responseFileText);

				var processResult = null as ProcessRunnerResult;

				using(var processRunner = new ProcessRunner(command, commandArgs))
				{
					processRunner.ProcessOutputRecevied += this.Process_OnOutputReceived;
					processRunner.Start();
					processResult = processRunner.WaitForExit();
				}

				if(processResult == null)
					throw new InvalidOperationException("wait for exit returned null");

				var exitCode = processResult.ExitCode;

				var isSuccess = exitCode == 0 && this.errorTextLines.Count == 0;

				var compilerResult = null as CSharpCompilerResult;

				sw.Stop();

				if (isSuccess)
				{
					compilerResult = CSharpCompilerResult.GetSuccess(exitCode,
						sw.Elapsed,
						this.outputTextLines,
						this.warningTextLines,
						args.OutputFilepath);
				}
				else
				{
					compilerResult = CSharpCompilerResult.GetFailure(exitCode,
						sw.Elapsed,
						null,
						this.outputTextLines,
						this.warningTextLines,
						this.errorTextLines);
				}

				return compilerResult;
			}
			catch (Exception ex)
			{
				throw CSharpCompilerInvocationException.Get(command, commandArgs, ex);
			}
			finally
			{
				if(File.Exists(responseFilename))
				{
					try
					{
						File.Delete(responseFilename);
					}
					catch(Exception)
					{
						
					}
				}

				this.outputTextLines.Clear();
				this.warningTextLines.Clear();
				this.errorTextLines.Clear();
			}
		}

		private void Process_OnOutputReceived(object sender, ProcessRunnerOutputReceivedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(e.Data))
				return;

			if (e.IsError)
			{
				this.errorTextLines.Add(e.Data);
			}
			else
			{
				var level = MSBuildHelper.GetOutputLevel(e.Data);

				switch(level)
				{
					case MSBuildOutputLevel.Normal:
						this.outputTextLines.Add(e.Data);
						break;

					case MSBuildOutputLevel.Error:
						this.errorTextLines.Add(e.Data);
						break;

					case MSBuildOutputLevel.Warning:
						this.warningTextLines.Add(e.Data);
						break;
				}
			}
		}

		private string GetCompilerArgString(CSharpCompilerArgs args)
		{
			var sb = new StringBuilder();

			var fixedSourceFiles = args.SourceFiles.Select(PathHelper.FixSlashesToOSFormat).ToList();
			var fixedReferenceFiles = args.ReferenceFiles.Select(PathHelper.FixSlashesToOSFormat).ToList();

			var output = this.GetOutputSwitch(args.OutputFilepath);
			var referenceFiles = this.GetReferenceFiles(fixedReferenceFiles);
			var sourceFiles = this.GetSourceFiles(fixedSourceFiles);

			sb.Append(output)
				.AppendLine()
				.Append(TargetSwitch)
				.AppendLine()
				.Append(OptimizeSwitch)
				.AppendLine()
				.Append(NoLogoSwitch);

			if (!string.IsNullOrWhiteSpace(referenceFiles))
				sb.AppendLine().Append(referenceFiles);

			if (!string.IsNullOrWhiteSpace(sourceFiles))
				sb.AppendLine().Append(sourceFiles);

			var text = sb.ToString();
			return text;
		}

		private string GetOutputSwitch(string outputFilepath)
		{
			var text = string.Format("{0}\"{1}\"", OutputSwitch, outputFilepath);
			return text;
		}

		private string GetSourceFiles(IList<string> items)
		{
			var sb = new StringBuilder();

			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];

				sb.Append("\"").Append(item).Append("\"");

				if(i != items.Count - 1)
					sb.AppendLine();
			}

			var text = sb.ToString();
			return text;
		}

		private string GetReferenceFiles(IList<string> items)
		{
			var sb = new StringBuilder();

			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];

				sb.Append(ReferenceSwitch)
					.Append("\"")
					.Append(item)
					.Append("\"");

				if (i != items.Count - 1)
					sb.AppendLine();
			}

			var text = sb.ToString();
			return text;
		}
	}
}
