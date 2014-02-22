using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone
{
	public class DroneTask
	{
		public string Name { get; private set; }
		public Action<DroneContext> Action { get; private set; }

		public DroneTask(string name, Action<DroneContext> action)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("name is empty or null", "name");

			if (action == null)
				throw new ArgumentNullException("action");

			this.Name = name;
			this.Action = action;
		}
	}
}
