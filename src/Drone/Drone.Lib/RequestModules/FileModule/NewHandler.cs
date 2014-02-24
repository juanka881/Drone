using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Drone.Lib.Repo;

namespace Drone.Lib.RequestModules.FileModule
{
	public class NewHandler : RequestHandler<NewParameter>
	{
		public override void HandleCore(NewParameter parameter)
		{
			if (File.Exists(parameter.DroneFilename))
				return;

			var currentDir = Directory.GetCurrentDirectory();
			var filepath = Path.Combine(currentDir, parameter.DroneFilename);
			var buildDir = Dronefile.DefaultBuildDir;

			var dronefile = new Dronefile(filepath, buildDir);

			this.Repo.Save(dronefile, filepath);
		}
	}
}