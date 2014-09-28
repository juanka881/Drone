using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Tasks
{
	public class AliasTask : DroneTask
	{
		public AliasTask()
		{
			
		}

		public AliasTask(string name, IEnumerable<string> deps) : base(name, deps)
		{
			
		}
	}
}