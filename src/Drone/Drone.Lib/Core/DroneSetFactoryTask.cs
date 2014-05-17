using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public class DroneSetFactoryTask : DroneTask
	{
		public Func<DroneTaskContext, IEnumerable<DroneTask>> Factory { get; private set; }

		public DroneSetFactoryTask(string name, IEnumerable<string> deps, Func<DroneTaskContext, IEnumerable<DroneTask>> factory)
			: base(name, deps)
		{
			if (factory == null)
				throw new ArgumentNullException("factory");

			this.Factory = factory;
		}
	}

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