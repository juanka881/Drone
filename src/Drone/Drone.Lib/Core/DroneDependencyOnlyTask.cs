using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public class DroneDependencyOnlyTask : DroneTask
	{
		public DroneDependencyOnlyTask()
		{
			
		}

		public DroneDependencyOnlyTask(string name, IEnumerable<string> deps) : base(name, deps)
		{
			
		}
	}
}