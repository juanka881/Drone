using Drone.Lib.Configs;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Core
{
	public abstract class RequestHandler
	{
		public Logger Log { get; set; }

		public DroneConfigRepo Repo { get; set; }

		public string DroneConfigFilepath { get; set; }

		public abstract void Handle(RequestTokens tokens);
	}
}