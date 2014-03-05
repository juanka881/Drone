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
			if (File.Exists(this.DroneConfigFilepath))
				return;

			var currentDir = Directory.GetCurrentDirectory();
			var filepath = Path.Combine(currentDir, this.DroneConfigFilepath);
			var buildDir = DroneConfig.DefaultBuildDir;

			var dronefile = new DroneConfig(filepath, buildDir);

			this.Repo.Save(dronefile, filepath);
		}
	}
}