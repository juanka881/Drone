using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Drone.Lib
{
	public class DroneFlags
	{
		public static DroneFlags Current { get; set; }

		public string ConfigFilename { get; set; }

		public bool IsDebugEnabled { get; set; }

		public bool IsConsoleLogColorsDisabled { get; set; }

		public LogLevel LogLevel { get; set; }
	}
}