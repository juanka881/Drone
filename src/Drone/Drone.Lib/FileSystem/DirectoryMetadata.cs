using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Drone.Lib.FileSystem
{
	public class DirectoryMetadata
	{
		public string FullPath { get; private set; }

		public string Dirname { get; private set; }

		public string OriginalDirname { get; private set; }

		public DirectoryMetadata(string dirname, string rootDir = null)
		{
			this.Set(dirname, rootDir);
		}

		private void Set(string dirname, string rootDir = null)
		{
			if(string.IsNullOrWhiteSpace(dirname))
				throw new ArgumentException("dirname is empty or null", "dirname");

			this.OriginalDirname = dirname;

			if(!string.IsNullOrWhiteSpace(rootDir) && Path.IsPathRooted(dirname))
				this.FullPath = Path.GetFullPath(Path.Combine(rootDir, dirname));
			else
				this.FullPath = Path.GetFullPath(dirname);

			this.Dirname = Path.GetFileName(this.FullPath);
		}
	}
}