using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Drone.API
{
	public class DroneContext
	{
		public string TaskName { get; private set; }

		public Logger Log { get; private set; }

		public DroneContext(string taskName, Logger log)
		{
			if (string.IsNullOrWhiteSpace(taskName))
				throw new ArgumentException("taskName is empty or null", "taskName");

			if (log == null) 
				throw new ArgumentNullException("log");

			this.TaskName = taskName;
			this.Log = log;
		}
	}
}