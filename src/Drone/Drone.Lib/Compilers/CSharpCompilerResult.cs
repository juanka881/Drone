using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Compilers
{
	public class CSharpCompilerResult
	{
		public bool IsSuccess { get; set; }

		public int ExiteCode { get; set; }

		public TimeSpan TimeElapsed { get; set; }

		public IList<string> OutputTextLines { get; set; }

		public IList<string> WarningTextLines { get; set; }

		public class SuccessResult : CSharpCompilerResult
		{
			public string OutputAssemblyFilepath { get; set; }
		}

		public class FailureResult : CSharpCompilerResult
		{
			public Exception Exception { get; set; }
			
			public IList<string> ErrorTextLines { get; set; }
		}

		public SuccessResult Success
		{
			get
			{
				return (SuccessResult) this;
			}
		}

		public FailureResult Failure
		{
			get
			{
				return (FailureResult) this;
			}
		}

		public static SuccessResult GetSuccess(int exitCode, 
			TimeSpan timeElapsed,
			IEnumerable<string> outputTextLines, 
			IEnumerable<string> warningTextLines, 
			string outputAssemblyFilepath)
		{

			if (outputTextLines == null)
				throw new ArgumentNullException("outputTextLines");

			if (warningTextLines == null)
				throw new ArgumentNullException("warningTextLines");

			if (string.IsNullOrWhiteSpace(outputAssemblyFilepath))
				throw new ArgumentException("outputAssemblyFilepath is empty or null", "outputAssemblyFilepath");

			var result = new SuccessResult();
			result.IsSuccess = true;
			result.ExiteCode = exitCode;
			result.TimeElapsed = timeElapsed;
			result.OutputTextLines = new List<string>(outputTextLines);
			result.WarningTextLines = new List<string>(warningTextLines);
			result.OutputAssemblyFilepath = outputAssemblyFilepath;

			return result;
		}

		public static FailureResult GetFailure(int exitCode,
			TimeSpan timeElapsed,
			Exception ex, 
			IEnumerable<string> outputTextLines, 
			IEnumerable<string> warningTextLines,
			IEnumerable<string> errorTextLines)
		{
			if (outputTextLines == null)
				throw new ArgumentNullException("outputTextLines");

			if (warningTextLines == null)
				throw new ArgumentNullException("warningTextLines");

			if (errorTextLines == null)
				throw new ArgumentNullException("errorTextLines");

			var result = new FailureResult();
			result.IsSuccess = false;
			result.ExiteCode = exitCode;
			result.TimeElapsed = timeElapsed;
			result.Exception = ex;
			result.OutputTextLines = new List<string>(outputTextLines);
			result.ErrorTextLines = new List<string>(errorTextLines);
			result.WarningTextLines = new List<string>(warningTextLines);

			return result;
		}
	}
}