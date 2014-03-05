using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public class TaskRunnerResult
	{
		public static readonly TaskRunnerResult Success = new TaskRunnerResult(null);

		public TaskRunnerResult(Exception ex)
		{
			this.Exception = ex;
		}

		public Exception Exception { get; private set; }

		public bool IsSuccess
		{
			get
			{
				return this.Exception == null;
			}
		}
	}
}