using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Drone.Lib.Helpers;

namespace Drone.Lib.Compilers
{
	public class CSharpCompiler
	{
		private static readonly string ReferenceSwitch = "/r:";
		private static readonly string OutputSwitch = "/out:";
		private static readonly string OptimizeSwitch = "/optimize";
		private static readonly string TargetSwitch = "/target:library";

		public CSharpCompilerResult Compile(CSharpCompilerArgs args)
		{
			if (args == null)
				throw new ArgumentNullException("args");

			var exitCode = 0;
			var outputText = string.Empty;
			var invocationError = null as Exception;
			var warnings = new List<string>();
			var errors = new List<string>();

			var framework = DotNetFramework.Version40;

			var compilerArgs = this.GetCompilerArgString(args);

			var psi = new ProcessStartInfo(framework.CSharpCompilerBinFilepath, compilerArgs);

			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			psi.WorkingDirectory = args.WorkDir;
			psi.CreateNoWindow = true;
			psi.WindowStyle = ProcessWindowStyle.Hidden;
			psi.UseShellExecute = false;

			var process = new Process();
			process.StartInfo = psi;

			process.ErrorDataReceived += (s, e) =>
			{
				Console.WriteLine(e.Data);
			};

			process.OutputDataReceived += (s, e) =>
			{
				Console.WriteLine(e.Data);
			};

			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();

			process.WaitForExit();

			process.CancelErrorRead();
			process.CancelOutputRead();

			exitCode = process.ExitCode;

			process.Dispose();
			process = null;

			return new CSharpCompilerResult(exitCode, 
				invocationError, 
				outputText, 
				args.OutputFilepath,
				warnings,
				errors);
		}

		private string GetCompilerArgString(CSharpCompilerArgs args)
		{
			var sb = new StringBuilder();

			var fixedSourceFiles = args.SourceFiles.Select(PathHelper.FixSlashesToOSFormat).ToList();
			var fixedReferenceFiles = args.ReferenceFiles.Select(PathHelper.FixSlashesToOSFormat).ToList();

			var output = this.GetOutputSwitch(args.OutputFilepath);
			var target = this.GetTargetSwitch();
			var optimize = this.GetOptimizeSwitch();
			var referenceFiles = this.GetReferenceFiles(fixedReferenceFiles);
			var sourceFiles = this.GetSourceFiles(fixedSourceFiles);

			sb.Append(output)
				.Append(" ")
				.Append(target)
				.Append(" ")
				.Append(optimize);

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

		private string GetTargetSwitch()
		{
			return TargetSwitch;
		}

		private string GetOptimizeSwitch()
		{
			return OptimizeSwitch;
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
