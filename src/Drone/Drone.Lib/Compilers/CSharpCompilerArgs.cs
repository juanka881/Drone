using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Compilers
{
	public class CSharpCompilerArgs
	{
		public string WorkDir { get; private set; }
		public string OutputFilepath { get; private set; }
		public IList<string> SourceFiles { get; private set; }
		public IList<string> ReferenceFiles { get; private set; }

		public CSharpCompilerArgs(
			string workDir,
			string outputFilepath,
			IEnumerable<string> sourceFiles, 
			IEnumerable<string> referenceFiles)
		{
			if (string.IsNullOrWhiteSpace(workDir))
				throw new ArgumentException("workDir is empty. workDir is expected to have a non-empty string value");

			if (string.IsNullOrWhiteSpace(outputFilepath))
				throw new ArgumentException("outputFilepath is empty. outputFilepath is expected to have a non-empty string value");

			if (sourceFiles == null)
				throw new ArgumentNullException("sourceFiles");

			if (referenceFiles == null)
				throw new ArgumentNullException("referenceFiles");

			this.WorkDir = workDir;
			this.OutputFilepath = outputFilepath;
			this.SourceFiles = new List<string>(sourceFiles);
			this.ReferenceFiles = new List<string>(referenceFiles);
		}
	}
}