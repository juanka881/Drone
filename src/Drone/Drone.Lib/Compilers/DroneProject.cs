using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Configs;

namespace Drone.Lib.Compilers
{
	public class DroneProject
	{
		public DroneProject(DroneConfig config)
		{
			if (config == null)
				throw new ArgumentNullException("config");

			
		}
	}
}