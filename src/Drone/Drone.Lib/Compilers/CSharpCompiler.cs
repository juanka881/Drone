using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Drone.Lib.Helpers;
using SlavaGu.ConsoleAppLauncher;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Drone.Lib.Compilers
{
	public class CSharpCompiler
	{
		private static readonly string NormalErrorPattern = @"(?<file>.*)\((?<line>\d+),(?<column>\d+)\):\s+(?<error>\w+)\s+(?<number>[\d\w]+):\s+(?<message>.*)";
		private static readonly string GeneralErrorPattern = @"(?<error>.+?)\s+(?<number>[\d\w]+?):\s+(?<message>.*)";

		private static readonly Regex NormalErrorRegex = new Regex(NormalErrorPattern, RegexOptions.Compiled);
		private static readonly Regex GeneralErrorRegex = new Regex(GeneralErrorPattern, RegexOptions.Compiled);

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

			var consoleApp = null as ConsoleApp;
			var command = string.Empty;
			var commandArgs = string.Empty;
			var sw = new Stopwatch();
			
			try
			{
				sw.Start();

				var framework = DotNetFramework.Version40;

				command = framework.CSharpCompilerBinFilepath;
				commandArgs = this.GetCompilerArgString(args);

				consoleApp = new ConsoleApp(command, commandArgs);

				consoleApp.ConsoleOutput += App_OnConsoleOutput;
				
				consoleApp.Run();
				consoleApp.WaitForExit();

				var method = typeof(ConsoleApp).GetMethod("DispatchProcessOutput", BindingFlags.Instance | BindingFlags.NonPublic);

				method.Invoke(consoleApp, null);
				
				if(consoleApp.ExitCode == null)
					throw ConsoleAppUnableToGetExitCodeException.Get(command, commandArgs);

				var exitCode = consoleApp.ExitCode.Value;

				var isSuccess = exitCode == 0 && this.errorTextLines.Count == 0;

				var result = null as CSharpCompilerResult;

				sw.Stop();

				if (isSuccess)
				{
					result = CSharpCompilerResult.GetSuccess(exitCode,
						sw.Elapsed,
						this.outputTextLines,
						this.warningTextLines,
						args.OutputFilepath);
				}
				else
				{
					result = CSharpCompilerResult.GetFailure(exitCode,
						sw.Elapsed,
						null,
						this.outputTextLines,
						this.warningTextLines,
						this.errorTextLines);
				}

				return result;
			}
			catch (Exception ex)
			{
				throw CSharpCompilerInvocationException.Get(command, commandArgs, ex);
			}
			finally
			{
				if (consoleApp != null)
					consoleApp.ConsoleOutput -= App_OnConsoleOutput;

				this.outputTextLines.Clear();
				this.warningTextLines.Clear();
				this.errorTextLines.Clear();
			}
		}

		private void App_OnConsoleOutput(object sender, ConsoleOutputEventArgs e)
		{
			if(string.IsNullOrWhiteSpace(e.Line))
				return;

			if(e.IsError)
			{
				this.errorTextLines.Add(e.Line);
				return;
			}
			
			var match = NormalErrorRegex.Match(e.Line);

			if(!match.Success)
			{
				match = GeneralErrorRegex.Match(e.Line);
			}

			if(match.Success)
			{
				var category = match.Groups["error"].Value;

				if (category == "warning")
				{
					this.warningTextLines.Add(e.Line);
				}
				else if (category == "error")
				{
					this.errorTextLines.Add(e.Line);
				}
			}
			else
			{
				this.outputTextLines.Add(e.Line);
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
				.Append(" ")
				.Append(TargetSwitch)
				.Append(" ")
				.Append(OptimizeSwitch)
				.Append(" ")
				.Append(NoLogoSwitch);

			if (!string.IsNullOrWhiteSpace(referenceFiles))
				sb.Append(" ").Append(referenceFiles);

			if (!string.IsNullOrWhiteSpace(sourceFiles))
				sb.Append(" ").Append(sourceFiles);

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

				if (i != items.Count - 1)
					sb.Append(" ");
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
					sb.Append(" ");
			}

			var text = sb.ToString();
			return text;
		}
	}
}
