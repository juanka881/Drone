using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib
{
	internal class DroneContextScope : IDisposable
	{
		public void Dispose()
		{
			DroneContext.Unset();
		}
	}
}