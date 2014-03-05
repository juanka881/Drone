using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Drone.Lib.Configs;
using Drone.Lib.Core;

namespace Drone.Lib.RequestHandlers
{
	public class InitHandler : RequestHandler
	{
		public override void Handle(RequestTokens tokens)
		{
			if (File.Exists(this.Flags.Filename))
				return;

			var currentDir = Directory.GetCurrentDirectory();

			var filepath = this.Flags.Filename;

			if(!Path.IsPathRooted(filepath))
				filepath = Path.Combine(currentDir, filepath);

			var buildDir = DroneConfig.DefaultBuildDir;

			var droneConfig = new DroneConfig(filepath, buildDir);

			this.Repo.Save(droneConfig);
		}
	}
}