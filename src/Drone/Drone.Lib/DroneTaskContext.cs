using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Core;
using NLog;

namespace Drone.Lib
{
	/// <summary>
	/// Represents the context in which a drone task is being executed.
	/// </summary>
	public class DroneTaskContext
	{
		private readonly Action<DroneTask, DroneConfig, DroneFlags> runTask;

		/// <summary>
		/// Initializes a new instance of the <see cref="DroneTaskContext" /> class.
		/// </summary>
		/// <param name="module">The module from where the task came from.</param>
		/// <param name="task">The task being executed.</param>
		/// <param name="config">The configuration loaded by the drone app.</param>
		/// <param name="flags">The flags.</param>
		/// <param name="log">The task log.</param>
		/// <param name="runTask">The run task function use to run other tasks.</param>
		/// <exception cref="System.ArgumentNullException">module
		/// or
		/// task
		/// or
		/// config
		/// or
		/// taskLog
		/// or
		/// runTask</exception>
		public DroneTaskContext(
			DroneModule module,
			DroneTask task, 
			DroneConfig config, 
			DroneFlags flags,
			Logger log, 
			Action<DroneTask, DroneConfig, DroneFlags> runTask)
		{
			if(module == null)
				throw new ArgumentNullException("module");

			if(task == null)
				throw new ArgumentNullException("task");

			if(config == null)
				throw new ArgumentNullException("config");

			if(flags == null)
				throw new ArgumentNullException("flags");

			if(log == null)
				throw new ArgumentNullException("log");

			if(runTask == null)
				throw new ArgumentNullException("runTask");

			this.Module = module;
			this.Task = task;
			this.Config = config;
			this.Flags = flags;
			this.Log = log;
			this.runTask = runTask;
		}

		/// <summary>
		/// Gets the module where the task is located
		/// </summary>
		/// <value>
		/// The module.
		/// </value>
		public DroneModule Module { get; private set; }

		/// <summary>
		/// Gets the task object
		/// </summary>
		/// <value>
		/// The task.
		/// </value>
		public DroneTask Task { get; private set; }

		/// <summary>
		/// Gets the drone app configuration.
		/// </summary>
		/// <value>
		/// The configuration.
		/// </value>
		public DroneConfig Config { get; private set; }

		/// <summary>
		/// Gets the drone app flags.
		/// </summary>
		/// <value>
		/// The flags.
		/// </value>
		public DroneFlags Flags { get; private set; }

		/// <summary>
		/// Gets the task log 
		/// </summary>
		/// <value>
		/// The log.
		/// </value>
		public Logger Log { get; private set; }

		/// <summary>
		/// Runs the specified task object.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <exception cref="System.ArgumentNullException">task</exception>
		public void Run(DroneTask task)
		{
			if (task == null)
				throw new ArgumentNullException("task");

			this.runTask(task, this.Config, this.Flags);
		}

		/// <summary>
		/// Runs the specified task, task is located by name in the task's module.
		/// </summary>
		/// <param name="taskName">Name of the task.</param>
		/// <exception cref="System.ArgumentException">taskName is empty or null;taskName</exception>
		public void Run(string taskName)
		{
			if(string.IsNullOrWhiteSpace(taskName))
				throw new ArgumentException("taskName is empty or null", "taskName");

			var task = this.Module.TryGet(taskName);

			if(task.HasValue)
			{
				this.Run(task.Value);
			}
			else
			{
				throw DroneTasksNotFoundException.Get(taskName);
			}
		}

		/// <summary>
		/// Causes the task being executed to fail. You can optionally pass a 
		/// message and exception related to the failure.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="ex">The exception that cause the failure.</param>
		public void Fail(string message = null, Exception ex = null)
		{
			if(!string.IsNullOrWhiteSpace(message))
				this.Log.Error(message);

			throw DroneTaskFailedException.Get(ex, message, this.Task.Name);
		}
	}
}