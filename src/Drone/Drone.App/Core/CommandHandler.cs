using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Configs;
using Drone.Lib.Core;
using Drone.Lib.Helpers;
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

		public DroneConfig Config { get; set; }

		public abstract void Handle(StringTokenSet tokens);

		protected CommandHandler()
		{
			this.Flags = new DroneFlags();
		}

		protected void SaveConfig(DroneConfig config)
		{
			this.Repo.Save(config);
		}

		protected DroneModule CompileAndLoadModule(DroneConfig config, LogLevel logLevel)
		{
			this.Compiler.Compile(config, logLevel);

			var module = this.Loader.Load(config);
			
			return module;
		}
	}
}