using Drone.Lib.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Tasks
{
	public class CopyFileTask : DroneTask
	{
		public FileSet Set { get; private set; }

		public string DestinationDir { get; set; }

		public CopyFileTask()
		{
			this.Set = new FileSet();
		}

		public CopyFileTask Include(string pattern)
		{
			this.Set.Include(pattern);
			return this;
		}

		public CopyFileTask Exclude(string pattern)
		{
			this.Set.Exclude(pattern);
			return this;
		}

		public CopyFileTask Dest(string destDir)
		{
			this.DestinationDir = destDir;
			return this;
		}

		public override DroneTask Clone(string newName)
		{
			return this.Clone(newName, x =>
			{
				x.Set = this.Set.Clone();
				x.DestinationDir = this.DestinationDir;
			});
		}
	}
}