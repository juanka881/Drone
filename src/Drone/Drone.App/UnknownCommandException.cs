using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.App
{
	[Serializable]
	public class UnknownCommandException : Exception
	{
		public UnknownCommandException()
		{
		}

		public UnknownCommandException(string message)
			: base(message)
		{
		}

		public UnknownCommandException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected UnknownCommandException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{

		}

		public static UnknownCommandException Get(string command)
		{
			var ex = new UnknownCommandException("unable to execute command. please see help for list of available commands");

			ex.Data["command"] = command ?? string.Empty;

			return ex;
		}
	}
}