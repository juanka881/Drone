using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Core
{
	[Serializable]
	public class DroneTaskFailedException : Exception
	{
		public DroneTaskFailedException()
		{
		}

		public DroneTaskFailedException(string message) : base(message)
		{
		}

		public DroneTaskFailedException(string message, Exception inner) : base(message, inner)
		{
		}

		protected DroneTaskFailedException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static DroneTaskFailedException Get(Exception ex, string message, string taskName)
		{
			var msg = "task failed";

			if(!string.IsNullOrWhiteSpace(message))
				msg = string.Format("task failed - {0}", message);

			var nex = new DroneTaskFailedException(msg, ex);

			nex.Data["task-name"] = taskName;
			nex.Data["has-exception"] = ex != null;

			return nex;
		}
	}
}