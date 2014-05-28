using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Helpers;

namespace Drone.Lib
{
	public class DroneTask
	{
		public string Name { get; private set; }

		public IList<string> Dependencies { get; private set; }

		public DroneTask()
		{
			this.Name = this.GetType().Name;
			this.Dependencies = new List<string>();
		}

		public DroneTask(string name, IEnumerable<string> deps)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("name is empty or null", "name");

			if(deps == null)
				throw new ArgumentNullException("deps");

			this.Name = name;
			this.Dependencies = new List<string>(deps);
		}

		public DroneTask(string name) : this(name, Enumerable.Empty<string>())
		{
			
		}

		public DroneTask CloneCore(string newName)
		{
			var task = Activator.CreateInstance(this.GetType()) as DroneTask;
			task.Name = newName;
			task.Dependencies.AddMany(this.Dependencies);
			return task;
		}

		public virtual DroneTask Clone(string newName)
		{
			return this.CloneCore(newName);
		}
	}
}
