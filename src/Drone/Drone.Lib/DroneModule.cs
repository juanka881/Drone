using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Helpers;

namespace Drone.Lib
{
	public class DroneModule
	{
		public static readonly string DefaultTaskName = "def";

		public static readonly string MainModuleName = "dronemain";

		private readonly IDictionary<string, DroneTask> tasks;

		public IEnumerable<DroneTask> Tasks
		{
			get
			{
				return this.tasks.Values;
			}
		}

		public int TaskCount
		{
			get
			{
				return this.tasks.Count;
			}
		}

		public DroneModule()
		{
			this.tasks = new Dictionary<string, DroneTask>();
		}

		public DroneTask TryGetTask(string name)
		{
			return this.tasks.GetOrDef(name);
		}

		public void Add(DroneTask task)
		{
			if(task == null)
				throw new ArgumentNullException("task");

			this.tasks[task.Name] = task;
		}

		public void Add(string ns, DroneModule module)
		{
			if (ns == null)
				throw new ArgumentNullException("ns");

			if (module == null)
				throw new ArgumentNullException("module");

			foreach (var task in module.Tasks)
			{
				var taskName = task.Name;

				if(!string.IsNullOrWhiteSpace(ns))
					taskName = string.Format("{0}/{1}", ns, task.Name);

				var newTask = task.Clone(taskName);

				var deps = new List<string>(newTask.Dependencies);
				newTask.Dependencies.Clear();

				foreach (var dep in deps)
				{
					var depName = string.Format("{0}/{1}", ns, dep);
					newTask.Dependencies.Add(depName);
				}

				this.Add(newTask);
			}
		}
	}
}