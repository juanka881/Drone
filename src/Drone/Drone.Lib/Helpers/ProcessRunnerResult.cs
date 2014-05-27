using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public class ProcessRunnerResult
	{
		public DateTime ExitTimestamp { get; private set; }

		public int ExitCode { get; private set; }

		public ProcessRunnerResult(DateTime exitTimestamp, int exitCode)
		{
			this.ExitTimestamp = exitTimestamp;
			this.ExitCode = exitCode;
		}
	}
}