using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Tasks
{
	public class CleanTask : DroneTask
	{
		public IList<string> Targets { get; set; }

		public CleanTask()
		{
			this.Targets = new List<string>();
		}

		public override DroneTask Clone(string newName)
		{
			return this.Clone(newName, x =>
			{
				x.Targets = new List<string>(this.Targets);
			});
		}
	}
}