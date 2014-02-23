using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.API.Core
{
	public class DroneRunnerResult
	{
		public static readonly DroneRunnerResult Success = new DroneRunnerResult(null);

		public DroneRunnerResult(Exception ex)
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