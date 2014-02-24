using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Drone.Lib.Repo
{
	public static class DronefileRepoExtensions
	{
		public static Dronefile Load(this DronefileRepo repo, string filepath)
		{
			if (repo == null)
				throw new ArgumentNullException("repo");

			using (var fs = File.OpenRead(filepath))
			{
				return repo.Load(fs, filepath);
			}
		}

		public static void Save(this DronefileRepo repo, Dronefile dronefile, string filepath)
		{
			if (repo == null)
				throw new ArgumentNullException("repo");

			using (var fs = File.Open(filepath, FileMode.Create, FileAccess.Write))
			{
				repo.Save(dronefile, fs);
			}
		}
	}
}