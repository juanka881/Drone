using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Exceptions
{
	[Serializable]
	public class TasksNotFoundException : Exception
	{
		public TasksNotFoundException()
		{
		}

		public TasksNotFoundException(string message) : base(message)
		{
		}

		public TasksNotFoundException(string message, Exception inner) : base(message, inner)
		{
		}

		protected TasksNotFoundException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static TasksNotFoundException Get(IEnumerable<string> taskNames)
		{
			var ex = new TasksNotFoundException("Unable to find tasks in module by name");

			ex.Data["task-names"] = string.Join(", ", taskNames);

			return ex;
		}
	}
}