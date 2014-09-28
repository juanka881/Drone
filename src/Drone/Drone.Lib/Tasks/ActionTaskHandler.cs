using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Tasks
{
	public class ActionTaskHandler : DroneTaskHandler<ActionTask>
	{
		public override void Handle(ActionTask task, DroneTaskContext context)
		{
			task.Action(context);
		}
	}
}