using Drone.Lib.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Drone.Lib.VSProjects
{
	public class CSProjectMetadata : FileMetadata
	{
		public CSProjectMetadata(string filePath) : base(filePath)
		{
			this.DebugDir = Path.Combine(this.FileDir, VSProject.BinDir, VSProject.DebugConfigName);
			this.ReleaseDir = Path.Combine(this.FileDir, VSProject.BinDir, VSProject.ReleaseConfigName);
		}

		public string DebugDir { get; private set; }

		public string ReleaseDir { get; private set; }
	}
}