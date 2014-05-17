using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public abstract class DroneTaskHandlerFactory
	{
		public abstract DroneTaskHandler TryGetHandler(DroneTask task);
	}
}