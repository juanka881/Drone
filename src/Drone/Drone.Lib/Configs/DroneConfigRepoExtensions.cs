using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Drone.Lib.Configs
{
	public static class DroneConfigRepoExtensions
	{
		public static DroneConfig Load(this DroneConfigRepo repo, string filepath)
		{
			if (repo == null)
				throw new ArgumentNullException("repo");

			using (var fs = File.OpenRead(filepath))
			{
				return repo.Load(fs, filepath);
			}
		}

		public static void Save(this DroneConfigRepo repo, DroneConfig droneConfig)
		{
			if (repo == null)
				throw new ArgumentNullException("repo");

			using (var fs = File.Open(droneConfig.FilePath, FileMode.Create, FileAccess.Write))
			{
				repo.Save(droneConfig, fs);
			}
		}
	}
}