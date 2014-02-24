using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Engine
{
	public class DroneTaskRunnerResult
	{
		public static readonly DroneTaskRunnerResult Success = new DroneTaskRunnerResult(null);

		public DroneTaskRunnerResult(Exception ex)
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