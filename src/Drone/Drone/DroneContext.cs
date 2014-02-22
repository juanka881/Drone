using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Drone
{
	public class DroneContext
	{
		public string TaskName { get; private set; }

		public Action<DroneContext> Action { get; private set; }
		public Logger Log { get; private set; }

		public DroneContext(string taskName, Action<DroneContext> action, Logger log)
		{
			if (string.IsNullOrWhiteSpace(taskName))
				throw new ArgumentException("taskName is empty or null", "taskName");

			if (action == null)
				throw new ArgumentNullException("action");

			if (log == null) 
				throw new ArgumentNullException("log");

			this.TaskName = taskName;
			this.Action = action;
			this.Log = log;
		}
	}
}