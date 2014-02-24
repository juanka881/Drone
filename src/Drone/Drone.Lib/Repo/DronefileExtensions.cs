using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Repo
{
	public static class DronefileExtensions
	{
		public static DronefileData ToDronefileData(this Dronefile dronefile)
		{
			if (dronefile == null)
				throw new ArgumentNullException("dronefile");

			var data = new DronefileData();
			data.SourceFiles = dronefile.SourceFiles;
			data.ReferenceFiles = dronefile.ReferenceFiles;
			data.BuildDir = dronefile.BuildDir;

			return data;
		}

		public static Dronefile ToDronefile(this DronefileData data, string filepath)
		{
			if (data == null)
				throw new ArgumentNullException("data");

			if (string.IsNullOrWhiteSpace(filepath))
				throw new ArgumentException("filepath is empty or null", "filepath");

			var dronefile = new Dronefile(filepath, data.BuildDir, data.SourceFiles, data.ReferenceFiles);
			
			return dronefile;
		}
	}
}