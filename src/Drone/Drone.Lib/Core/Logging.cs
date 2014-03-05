using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public class Logging
	{
		public static readonly string LogName = "drone";
		public static readonly string TaskLogBaseName = "drone.task";

		public static string GetTaskLogName(string taskName)
		{
			if (string.IsNullOrWhiteSpace(taskName))
				throw new ArgumentException("taskName is empty or null", "taskName");

			return string.Format("{0}.{1}", TaskLogBaseName, taskName);
		}
	}
}