using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Configs;
using Drone.Lib.Core;
using NLog;
using Drone.Lib;

namespace Drone.App.Core
{
	public abstract class CommandHandler
	{
		public Logger Log { get; set; }

		public DroneConfigRepo Repo { get; set; }

		public DroneCompiler Compiler { get; set; }

		public DroneLoader Loader { get; set; }

		public DroneFlags Flags { get; set; }

		public abstract void Handle(StringTokenSet tokens);

		protected CommandHandler()
		{
			this.Flags = new DroneFlags();
		}

		protected void SaveConfig(DroneConfig config)
		{
			this.Repo.Save(config);
		}

		protected DroneConfig LoadConfig()
		{
			return this.Repo.Load(this.Flags.ConfigFilename);
		}

		protected DroneModule CompileAndLoadModule(DroneConfig config)
		{
			this.Compiler.Compile(config);

			var module = this.Loader.Load(config);
			
			return module;
		}
	}
}