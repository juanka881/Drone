using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Core;
using NLog;
using Drone.Lib.Configs;

namespace Drone.Lib
{
	public class DroneTaskContext
	{
		public string TaskName { get; private set; }

		public Logger Log { get; private set; }

		public DroneConfig Config { get; private set; }

		public DroneTaskContext(string name, Logger log, DroneConfig config)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("name is empty or null", "name");

			if (log == null) 
				throw new ArgumentNullException("log");

			if(config == null)
				throw new ArgumentNullException("config");

			this.TaskName = name;
			this.Log = log;
			this.Config = config;
		}

		public void Cancel(Exception ex = null)
		{
			throw DroneTaskCancelException.Get(ex, this.TaskName);
		}
	}
}