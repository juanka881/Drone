using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public class DroneSetFactoryTaskHandler : DroneTaskHandler<DroneSetFactoryTask>
	{
		public override void Handle(DroneSetFactoryTask task, DroneTaskContext context)
		{
			var set = task.Factory(context);
			var e = set.GetEnumerator();
			while(e.MoveNext())
			{
				context.Run(e.Current);
			}
		}
	}
}