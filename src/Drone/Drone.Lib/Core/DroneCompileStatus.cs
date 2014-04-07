using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public class DroneCompileStatus
	{
		public bool HasError { get; set; }

		public IDictionary<string, DateTime> FileTimestamps { get; private set; }

		public DroneCompileStatus()
		{
			this.FileTimestamps = new Dictionary<string, DateTime>(); 
		}
	}
}