using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Drone.Lib.Exceptions
{
	[Serializable]
	public class CommandHandlerNotFoundException : Exception
	{
		public CommandHandlerNotFoundException()
		{
		}

		public CommandHandlerNotFoundException(string message) : base(message)
		{
		}

		public CommandHandlerNotFoundException(string message, Exception inner) : base(message, inner)
		{
		}

		protected CommandHandlerNotFoundException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static CommandHandlerNotFoundException Get(string command)
		{
			var ex = new CommandHandlerNotFoundException("Unable to get handler for command");

			ex.Data["command"] = command ?? string.Empty;
			
			return ex;
		}
	}
}