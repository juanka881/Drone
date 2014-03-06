using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Drone.Lib.Configs;
using Drone.Lib.Core;

namespace Drone.Lib.CommandHandlers
{
	public class InitHandler : CommandHandler
	{
		public override void Handle(CommandTokens tokens)
		{
			if (File.Exists(this.Flags.ConfigFilename))
			{
				this.Log.Warn("file '{0}' already exists", this.Flags.ConfigFilename);
				return;
			}

			var droneConfig = new DroneConfig(this.Flags.ConfigFilename, DroneConfig.DefaultBuildDir);

			this.SaveConfig(droneConfig);

			this.Log.Info("created '{0}'", this.Flags.ConfigFilename);
		}
	}
}