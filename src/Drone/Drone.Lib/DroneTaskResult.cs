using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib
{
	public class DroneTaskResult
	{
		public DroneTask Task { get; private set; }

		public DroneTaskState State { get; private set; }

		public TimeSpan TimeElapsed { get; private set; }
		
		public Exception Exception { get; private set; }

		public bool IsSuccess
		{
			get
			{
				return this.Exception == null && this.State == DroneTaskState.Completed;
			}
		}

		public DroneTaskResult(DroneTask task, DroneTaskState state, TimeSpan ts)
		{
			if(task == null)
				throw new ArgumentNullException("task");

			this.Task = task;
			this.State = state;
			this.Exception = null;
			this.TimeElapsed = ts;
		}

		public DroneTaskResult(DroneTask task, DroneTaskState state, TimeSpan ts, Exception ex)
		{
			if(task == null)
				throw new ArgumentNullException("task");

			if(ex == null)
				throw new ArgumentNullException("ex");

			this.Task = task;
			this.State = state;
			this.Exception = ex;
			this.TimeElapsed = ts;
		}
	}
}