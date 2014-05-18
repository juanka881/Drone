using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public class DroneDependencyOnlyTaskHandler : DroneTaskHandler<DroneDependencyOnlyTask>
	{
		public override void Handle(DroneDependencyOnlyTask task, DroneTaskContext context)
		{
			foreach(var taskName in task.Dependencies)
				context.Run(taskName);
		}
	}
}