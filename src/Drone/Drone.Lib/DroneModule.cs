using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Helpers;

namespace Drone.Lib
{
	/// <summary>
	/// represent a task module. a module is a grouping unit for task. 
	/// a module can be merge into another module one or more times 
	/// every time a module is merge into another module the modules task 
	/// are clone and have the option to be register on the module with a new namespace.
	/// namespace is just a string prefix added to the name of the task to prevent 
	/// collisions when modules define task with the same name. 
	/// </summary>
	public class DroneModule
	{
		/// <summary>
		/// The default task name for a drone module. drone app
		/// search for this task when no task is specified in the list 
		/// of tasks to execute
		/// </summary>
		public static readonly string DefaultTaskName = "def";

		/// <summary>
		/// The main module name. drone app searchs for a type that 
		/// derives from drone module and its called 'MainModule' casing
		/// does not matter. This main module is use as the entry point
		/// when locating task to execute. Having more than 1 main module will 
		/// cause an error. 
		/// </summary>
		public static readonly string MainModuleName = "main";

		private readonly IDictionary<string, DroneTask> tasks;

		/// <summary>
		/// Gets the tasks registered on this module
		/// </summary>
		/// <value>
		/// The tasks.
		/// </value>
		public IEnumerable<DroneTask> Tasks
		{
			get
			{
				return this.tasks.Values;
			}
		}

		/// <summary>
		/// Gets the number of task in this module.
		/// </summary>
		/// <value>
		/// The task count.
		/// </value>
		public int Count
		{
			get
			{
				return this.tasks.Count;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DroneModule"/> class.
		/// </summary>
		public DroneModule()
		{
			this.tasks = new Dictionary<string, DroneTask>();
		}

		/// <summary>
		/// Tries to get the task with the given name.
		/// </summary>
		/// <param name="name">The name of the task.</param>
		/// <returns>an option of drone task</returns>
		public Option<DroneTask> TryGet(string name)
		{
			return this.tasks.Get(name);
		}

		/// <summary>
		/// Adds the given task to this module
		/// </summary>
		/// <param name="task">The task.</param>
		/// <exception cref="System.ArgumentNullException">task</exception>
		public void Add(DroneTask task)
		{
			if(task == null)
				throw new ArgumentNullException("task");

			this.tasks[task.Name] = task;
		}

		/// <summary>
		/// Includes the module into this module prefixing any
		/// updating task names with the given namespace if any.
		/// </summary>
		/// <param name="ns">The namespace to prefix on the tasks included.</param>
		/// <param name="module">The module to include.</param>
		/// <exception cref="System.ArgumentNullException">
		/// ns
		/// or
		/// module
		/// </exception>
		public void Include(string ns, DroneModule module)
		{
			if (ns == null)
				throw new ArgumentNullException("ns");

			if (module == null)
				throw new ArgumentNullException("module");

			var hasNamespace = string.IsNullOrWhiteSpace(ns);

			foreach (var task in module.Tasks)
			{
				var newTaskName = task.Name;
				var newTaskDeps = task.Dependencies;

				if(hasNamespace)
				{
					newTaskName = string.Format("{0}/{1}", ns, task.Name);
					newTaskDeps = task.Dependencies.Select(x => string.Format("{0}/{1}", ns, x)).ToList();
				}

				var newTask = task.Clone(newTaskName);
				newTask.Dependencies = newTaskDeps;

				this.Add(newTask);
			}
		}
	}
}