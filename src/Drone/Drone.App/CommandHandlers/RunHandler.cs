using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.App.Core;
using Drone.Lib;
using Drone.Lib.Core;
using Drone.Lib.Helpers;
using NLog;

namespace Drone.App.CommandHandlers
{
	public class RunHandler : CommandHandler
	{
		public DroneTaskRunner Runner { get; set; }

		public override void Handle(StringTokenSet tokens)
		{
			var config = this.LoadConfig();

			DroneConfig.Current = config;

			var module = this.CompileAndLoadModule(config, LogLevel.Debug);

			var taskNames = tokens.Where(x => !x.Value.StartsWith("-")).Select(x => x.Value);

			this.Runner.Run(module, taskNames, config);
		}
	}
}