using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public class DroneFactoryTaskHandler : DroneTaskHandler<DroneFactoryTask>
	{
		public override void Handle(DroneFactoryTask task, DroneTaskContext context)
		{
			var resultTask = task.Factory(context);
			context.Run(resultTask);
		}
	}
}