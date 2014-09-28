using Drone.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib
{
	public class DroneTaskResult
	{
		public Option<DroneTask> Task { get; private set; }

		public DroneTaskState State { get; private set; }

		public TimeSpan TimeElapsed { get; private set; }
		
		public Option<Exception> Exception { get; private set; }

		public bool IsSuccess
		{
			get
			{
				return !this.Exception.HasValue && this.State == DroneTaskState.Completed;
			}
		}

		public DroneTaskResult(Option<DroneTask> task, DroneTaskState state, TimeSpan ts, Option<Exception> ex)
		{
			this.Task = task;
			this.State = state;
			this.Exception = ex;
			this.TimeElapsed = ts;
		}
	}
}