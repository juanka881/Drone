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

		public static string GetRelativePathToBasePath(string basePath, string filePath)
		{
			if(string.IsNullOrWhiteSpace(basePath))
				throw new ArgumentException("basePath is empty or null", "basePath");

			if(string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("filePath is empty or null", "filePath");

			var baseUri = new Uri(basePath, UriKind.RelativeOrAbsolute);
			var fileUri = new Uri(filePath, UriKind.RelativeOrAbsolute);

			var relUri = baseUri.MakeRelativeUri(fileUri);
			var relPath = FixSlashesToOSFormat(Uri.UnescapeDataString(relUri.ToString()));
			return relPath;
		}
	}
}