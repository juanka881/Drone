using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Drone.App.Core;
using Drone.Lib;
using Drone.Lib.Core;

namespace Drone.App.CommandHandlers
{
	public class InitHandler : CommandHandler
	{
		public override void Handle(StringTokenSet tokens)
		{
			if (File.Exists(this.Flags.ConfigFilename))
			{
				this.Log.Warn("file '{0}' already exists", this.Flags.ConfigFilename);
				return;
			}

			var config = new DroneConfig();
			config.SetConfigFilename(this.Flags.ConfigFilename);

			this.SaveConfig(config);

			this.Log.Info("created '{0}'", this.Flags.ConfigFilename);
		}
	}
}