using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Configs
{
	public static class DroneConfigExtensions
	{
		public static DroneConfigData ToDroneConfigData(this DroneConfig droneConfig)
		{
			if (droneConfig == null)
				throw new ArgumentNullException("droneConfig");

			var data = new DroneConfigData();
			data.SourceFiles = droneConfig.SourceFiles;
			data.ReferenceFiles = droneConfig.ReferenceFiles;
			data.BuildDir = droneConfig.BuildDir;

			return data;
		}

		public static DroneConfig ToDroneConfig(this DroneConfigData data, string filepath)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			if (string.IsNullOrWhiteSpace(filepath))
				throw new ArgumentException("filepath is empty or null", "filepath");

			var dronefile = new DroneConfig(filepath, data.BuildDir, data.SourceFiles, data.ReferenceFiles);
			
			return dronefile;
		}
	}
}