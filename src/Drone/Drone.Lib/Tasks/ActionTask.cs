using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Tasks
{
	public class ActionTask : DroneTask
	{
		public Action<DroneTaskContext> Action { get; private set; }

		public ActionTask()
		{
			
		}

		public ActionTask(string name, IEnumerable<string> deps, Action<DroneTaskContext> action) 
			: base(name, deps)
		{
			if(action == null)
				throw new ArgumentNullException("action");

			this.Action = action;
		}

		public override DroneTask Clone(string newName)
		{
			return this.Clone(newName, x => x.Action = this.Action);
		}
	}
}