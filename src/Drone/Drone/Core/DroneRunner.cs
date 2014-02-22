using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Core
{
	public class DroneRunner
	{
		public void Run(DroneModule module, IEnumerable<string> taskNames)
		{
			if (module == null)
				throw new ArgumentNullException("module");

			if (taskNames == null)
				throw new ArgumentNullException("taskNames");

			throw new NotImplementedException();
		}
	}
}
