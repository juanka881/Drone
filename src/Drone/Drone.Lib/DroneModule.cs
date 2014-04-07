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

		public void Add(string name, IEnumerable<string> deps)
		{
			this.Add(name, c => { }, deps);
		}

		public void Add(string name, params string[] deps)
		{
			this.Add(name, c => { }, deps);
		}

		public void Add(string name, Action<DroneTaskContext> fn)
		{
			this.Add(name, fn, Enumerable.Empty<string>());
		}

		public void Add(string name, Action<DroneTaskContext> fn, params string[] deps)
		{
			this.Add(name, fn, deps as IEnumerable<string>);
		}

		public void Add(string name, Action<DroneTaskContext> fn, IEnumerable<string> deps)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("name is empty or null", "name");

			if(deps == null)
				throw new ArgumentNullException("deps");

			if(fn == null)
				throw new ArgumentNullException("fn");

			var task = new DroneTask(name, deps, fn);
			this.Add(task);
		}

		public void Add(DroneTask task)
		{
			if(task == null)
				throw new ArgumentNullException("task");

			this.tasks[task.Name] = task;
		}

		public void Add(DroneModule module)
		{
			this.Add(string.Empty, module);
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

				var newTask = new DroneTask(taskName, task.Dependencies, task.Action);
				this.Add(newTask);
			}
		}
	}
}