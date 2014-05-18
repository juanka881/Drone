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

		public override DroneTask Clone(string newName)
		{
			return this.Clone(newName, x => x.Factory = this.Factory);
		}
	}
}