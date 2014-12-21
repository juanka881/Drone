using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Drone.Lib.FileSystem
{
	public class DirMetadata
	{
		public string DirPath { get; private set; }

		public string DirName { get; private set; }

		public string OriginalValue { get; private set; }

		public bool IsRelative { get; private set; }

		public DirMetadata(string dirPath)
		{
			if (string.IsNullOrWhiteSpace(dirPath))
				throw new ArgumentException("dirPath is empty or null", "dirPath");

			this.OriginalValue = dirPath;
			this.IsRelative = !Path.IsPathRooted(dirPath);
			this.DirPath = Path.GetFullPath(dirPath);
			this.DirName = Path.GetDirectoryName(this.DirPath);
		}
	}
}