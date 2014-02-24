using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Compilers
{
	public class DronefileCompilerResult
	{
		public string OutputFilepath { get; private set; }

		public DronefileCompilerResult(string outputFilepath)
		{
			if (string.IsNullOrWhiteSpace(outputFilepath))
				throw new ArgumentException("outputFilepath is empty. outputFilepath is expected to have a non-empty string value");

			this.OutputFilepath = outputFilepath;
		}
	}
}