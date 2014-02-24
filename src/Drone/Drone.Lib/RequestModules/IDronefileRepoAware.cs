using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Repo;

namespace Drone.Lib.RequestModules
{
	public interface IDronefileRepoAware
	{
		DronefileRepo Repo { get; set; }
	}
}