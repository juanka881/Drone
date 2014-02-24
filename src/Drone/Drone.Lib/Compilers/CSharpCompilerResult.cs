using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Compilers
{
	public class CSharpCompilerResult
	{
		public bool Success
		{
			get
			{
				return this.InvocationError == null && this.Errors.Count == 0 && this.ExiteCode == 0;
			}
		}

		public int ExiteCode { get; private set; }
		public Exception InvocationError { get; private set; }

		public string OutputText { get; private set; }
		public string OutputAssemblyFilepath { get; private set; }

		public IList<string> Warnings { get; private set; }
		public IList<string> Errors { get; private set; }

		public CSharpCompilerResult(int exitCode, 
			Exception invocationError, 
			string outputText,
			string outputAssemblyFile,
			IEnumerable<string> warnings,
			IEnumerable<string> errors)
		{
			if (string.IsNullOrWhiteSpace(outputAssemblyFile))
				throw new ArgumentException("OutputAssemblyFile is empty. OutputAssemblyFile is expected to have a non-empty string value");

			if (warnings == null)
				throw new ArgumentNullException("warnings");

			if (errors == null)
				throw new ArgumentNullException("errors");

			this.ExiteCode = exitCode;
			this.InvocationError = invocationError;
			this.OutputText = outputText;
			this.OutputAssemblyFilepath = outputAssemblyFile;
			this.Warnings = new List<string>(warnings);
			this.Errors = new List<string>(errors);
		}
	}
}