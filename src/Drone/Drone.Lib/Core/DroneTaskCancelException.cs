using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Core
{
	[Serializable]
	public class DroneTaskCancelException : Exception
	{
		public DroneTaskCancelException()
		{
		}

		public DroneTaskCancelException(string message) : base(message)
		{
		}

		public DroneTaskCancelException(string message, Exception inner) : base(message, inner)
		{
		}

		protected DroneTaskCancelException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static DroneTaskCancelException Get(Exception ex, string taskName)
		{
			var msg = "cancel was requested on task";
			var nex = new DroneTaskCancelException(msg, ex);

			nex.Data["task-name"] = taskName;
			nex.Data["has-exception"] = ex != null;

			return nex;
		}
	}
}