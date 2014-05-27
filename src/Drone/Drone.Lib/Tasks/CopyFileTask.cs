using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Tasks
{
	public class CopyFileTask : DroneTask
	{
		public IList<string> Sources { get; set; }

		public string Destination { get; set; }

		public CopyFileTask()
		{
			this.Sources = new List<string>();
		}

		public override DroneTask Clone(string newName)
		{
			return this.Clone(newName, x =>
			{
				x.Sources = new List<string>(this.Sources);
				x.Destination = this.Destination;
			});
		}
	}
}