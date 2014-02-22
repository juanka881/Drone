using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Helpers;

namespace Drone
{
	public class DroneModule
	{
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

		public void Register(string name, Action<DroneContext> action)
		{
			var task = new DroneTask(name, action);
			this.Register(task);
		}

		public void Register(DroneTask task)
		{
			this.tasks[task.Name] = task;
		}

		public void Register(DroneModule module)
		{
			this.Register(string.Empty, module);
		}

		public void Register(string ns, DroneModule module)
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

				var newTask = new DroneTask(taskName, task.Action);
				this.Register(newTask);
			}
		}
	}
}