using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public class DroneActionTask : DroneTask
	{
		public Action<DroneTaskContext> Action { get; private set; }

		public DroneActionTask(string name, IEnumerable<string> deps, Action<DroneTaskContext> action) 
			: base(name, deps)
		{
			if(action == null)
				throw new ArgumentNullException("action");

			this.Action = action;
		}
	}
}