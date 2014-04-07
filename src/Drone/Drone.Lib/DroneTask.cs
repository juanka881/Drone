using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib
{
	public class DroneTask
	{
		public string Name { get; private set; }

		public IList<string> Dependencies { get; private set; }

		public Action<DroneTaskContext> Action { get; private set; }

		public DroneTask(string name, IEnumerable<string> deps, Action<DroneTaskContext> action)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("name is empty or null", "name");

			if(action == null)
				throw new ArgumentNullException("action");

			if(deps == null)
				throw new ArgumentNullException("deps");

			this.Name = name;
			this.Dependencies = new List<string>(deps);
			this.Action = action;
		}
	}
}
