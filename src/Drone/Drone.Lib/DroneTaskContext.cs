using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Core;
using NLog;

namespace Drone.Lib
{
	public delegate void DroneTaskRunner(DroneTask task, DroneEnv env);

	/// <summary>
	/// Represents the context in which a drone task is being executed.
	/// </summary>
	public class DroneTaskContext
	{
		private readonly DroneTaskRunner taskRunner;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DroneTaskContext" /> class.
		/// </summary>
		/// <param name="module">The module from where the task came from.</param>
		/// <param name="task">The task being executed.</param>
		/// <param name="env">The env.</param>
		/// <param name="log">The task log.</param>
		/// <param name="taskRunner">The run task function use to run other tasks.</param>
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
			DroneEnv env,
			Logger log, 
			DroneTaskRunner taskRunner)
		{
			if(module == null)
				throw new ArgumentNullException("module");

			if(task == null)
				throw new ArgumentNullException("task");

			if(env == null)
				throw new ArgumentNullException("env");

			if(log == null)
				throw new ArgumentNullException("log");

			if(taskRunner == null)
				throw new ArgumentNullException("taskRunner");

			this.Module = module;
			this.Task = task;
			this.Env = env;
			this.Log = log;
			this.taskRunner = taskRunner;
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
		/// Gets the environment for the current task.
		/// </summary>
		/// <value>
		/// The env.
		/// </value>
		public DroneEnv Env { get; private set; }

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

			this.taskRunner(task, this.Env);
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