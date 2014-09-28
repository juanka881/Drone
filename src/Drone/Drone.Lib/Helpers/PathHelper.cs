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
			var fixedPath = path;

			switch(Environment.OSVersion.Platform)
			{
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.Win32NT:
				case PlatformID.WinCE:
				case PlatformID.Xbox:
					fixedPath = FixSlashesToWindowsFormat(path);
					break;

				case PlatformID.Unix:
				case PlatformID.MacOSX:
					fixedPath = FixSlashesToPosixFormat(path);
					break;
			}

			return fixedPath;
		}

		public static string FixSlashesToWindowsFormat(string path)
		{
			var fixedPath = path.Replace("/", "\\");
			return fixedPath;
		}

		public static string FixSlashesToPosixFormat(string path)
		{
			var fixedPath = path.Replace("\\", "/");
			return fixedPath;
		}
	}
}