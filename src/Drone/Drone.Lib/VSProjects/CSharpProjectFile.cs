using Drone.Lib.FileSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Drone.Lib.VSProjects
{
	public class CSharpProjectMetadata : FileMetadata
	{
		public CSharpProjectMetadata(string filename, string rootDir = null) : base(filename, rootDir)
		{
		}

		public string DebugDir { get; set; }

		public string ReleaseDir { get; set; }

		protected override void OnSet()
		{
			this.DebugDir = Path.Combine(this.FileDir, VSProject.BinDir, VSProject.DebugConfigName);
			this.ReleaseDir = Path.Combine(this.FileDir, VSProject.BinDir, VSProject.ReleaseConfigName);
		}
	}
}