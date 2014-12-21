using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Tasks
{
	public class StartProcessTask : DroneTask
	{
		public string FileName { get; set; }

		public string Args { get; set; }

		public string WorkDir { get; set; }

		public override DroneTask Clone(string newName)
		{
			return this.Clone(newName, x =>
			{
				x.FileName = this.FileName;
				x.Args = this.Args;
				x.WorkDir = this.WorkDir;
			});
		}
	}
}