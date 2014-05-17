using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public class DroneActionTaskHandler : DroneTaskHandler<DroneActionTask>
	{
		public override void Handle(DroneActionTask task, DroneTaskContext context)
		{
			task.Action(context);
		}
	}
}