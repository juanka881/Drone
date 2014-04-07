using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Drone.App.Core
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
			var ex = new CommandHandlerNotFoundException("unable to get handler for given command");

			ex.Data["command"] = command ?? string.Empty;
			
			return ex;
		}
	}
}