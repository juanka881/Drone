using Drone.Lib.Configs;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Core
{
	public abstract class CommandHandler
	{
		public Logger Log { get; set; }

		public DroneConfigRepo Repo { get; set; }

		public DroneFlags Flags { get; set; }

		public abstract void Handle(CommandTokens tokens);

		public void SaveConfig(DroneConfig config)
		{
			this.Repo.Save(config);
		}

		public DroneConfig LoadConfig()
		{
			return this.Repo.Load(this.Flags.ConfigFilename);
		}

		protected CommandHandler()
		{
			this.Flags = new DroneFlags();
		}
	}
}