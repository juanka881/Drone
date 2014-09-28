using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Tasks
{
	public class AliasTaskHandler : DroneTaskHandler<AliasTask>
	{
		public override void Handle(AliasTask task, DroneTaskContext context)
		{
			foreach(var taskName in task.Dependencies)
				context.Run(taskName);
		}
	}
}