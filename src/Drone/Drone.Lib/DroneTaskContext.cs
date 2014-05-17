using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Configs;
using NLog;

namespace Drone.Lib
{
	public class DroneTaskContext
	{
		private readonly Action<DroneTask, DroneConfig> runTask;

		public DroneTaskContext(
			DroneModule module,
			DroneTask task, 
			DroneConfig config, 
			Logger taskLog, 
			Action<DroneTask, DroneConfig> runTask)
		{
			if(module == null)
				throw new ArgumentNullException("module");

			if(task == null)
				throw new ArgumentNullException("task");

			if(config == null)
				throw new ArgumentNullException("config");

			if(taskLog == null)
				throw new ArgumentNullException("taskLog");

			if(runTask == null)
				throw new ArgumentNullException("runTask");

			this.Module = module;
			this.Task = task;
			this.Config = config;
			this.Log = taskLog;
			this.runTask = runTask;
		}

		public DroneModule Module { get; private set; }

		public DroneTask Task { get; private set; }

		public DroneConfig Config { get; private set; }

		public Logger Log { get; private set; }

		public void Run(DroneTask task)
		{
			this.runTask(task, this.Config);
		}

		public void Run(string taskName)
		{
			this.Run(this.Module.TryGetTask(taskName));
		}
	}
}