using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Compilers
{
	public class DroneProjectCompilerResult
	{
		public class Success : DroneProjectCompilerResult
		{
			public string OutputFilepath { get; set; }
		}

		public class Failure : DroneProjectCompilerResult
		{
			public Exception Exception { get; set; }
		}

		public bool IsSuccess { get; private set; }

		public static DroneProjectCompilerResult GetFailure(Exception ex)
		{
			if (ex == null)
				throw new ArgumentNullException("ex");

			return new Failure
			{
				IsSuccess = false,
				Exception = ex
			};
		}
		
		public static DroneProjectCompilerResult GetSuccess(string outputFilepath)
		{
			if (outputFilepath == null)
				throw new ArgumentNullException("outputFilepath");

			return new Success
			{
				IsSuccess = true,
				OutputFilepath = outputFilepath
			};
		}
	}
}