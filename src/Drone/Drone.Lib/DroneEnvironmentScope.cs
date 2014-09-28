using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib
{
	internal class DroneEnvironmentScope : IDisposable
	{
		public void Dispose()
		{
			DroneEnvironment.Flags = null;
			DroneEnvironment.Config = null;
		}
	}
}