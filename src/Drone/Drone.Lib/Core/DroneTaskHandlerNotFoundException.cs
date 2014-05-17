using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Core
{
	[Serializable]
	public class DroneTaskHandlerNotFoundException : Exception
	{
		public DroneTaskHandlerNotFoundException()
		{
		}

		public DroneTaskHandlerNotFoundException(string message) : base(message)
		{
		}

		public DroneTaskHandlerNotFoundException(string message, Exception inner) : base(message, inner)
		{
		}

		protected DroneTaskHandlerNotFoundException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static DroneTaskHandlerNotFoundException Get(DroneTask task)
		{
			if(task == null)
				throw new ArgumentNullException("task");

			var ex = new DroneTaskHandlerNotFoundException("unable to find handler for task");
			ex.Data["task-is-null"] = task == null;
			ex.Data["task-type"] = task == null ? string.Empty : task.GetType().FullName;

			return ex;
		}
	}
}