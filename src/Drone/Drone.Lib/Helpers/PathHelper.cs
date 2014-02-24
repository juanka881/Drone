using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Helpers
{
	public class PathHelper
	{
		public static string FixSlashesToOSFormat(string path)
		{
			var fixedPath = FixSlashesToWindowsFormat(path);
			return fixedPath;
		}

		public static string FixSlashesToWindowsFormat(string path)
		{
			var fixedPath = path.Replace("/", "\\");
			return fixedPath;
		}
	}
}