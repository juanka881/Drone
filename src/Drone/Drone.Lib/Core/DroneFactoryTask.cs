using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public class DroneFactoryTask : DroneTask
	{
		public Func<DroneTaskContext, DroneTask> Factory { get; private set; }

		public DroneFactoryTask(string name, IEnumerable<string> deps, Func<DroneTaskContext, DroneTask> factory)
			: base(name, deps)
		{
			if (factory == null)
				throw new ArgumentNullException("factory");

			this.Factory = factory;
		}
	}
}