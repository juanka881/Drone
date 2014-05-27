using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Tasks
{
	public class RunProcessTaskResult
	{
		public bool ErrorOutputReceived { get; private set; }

		public int ExitCode { get; private set; }

		public bool IsSuccess
		{
			get
			{
				return this.ExitCode != 0 || this.ErrorOutputReceived;
			}
		}

		public RunProcessTaskResult(bool errorOutputReceived, int exitCode)
		{
			this.ErrorOutputReceived = errorOutputReceived;
			this.ExitCode = exitCode;
		}
	}
}