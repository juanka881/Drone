using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Compilers
{
	[Serializable]
	public class ConsoleAppUnableToGetExitCodeException : Exception
	{
		public ConsoleAppUnableToGetExitCodeException()
		{
		}

		public ConsoleAppUnableToGetExitCodeException(string message) : base(message)
		{
		}

		public ConsoleAppUnableToGetExitCodeException(string message, Exception inner) : base(message, inner)
		{
		}

		protected ConsoleAppUnableToGetExitCodeException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{

		}

		public static ConsoleAppUnableToGetExitCodeException Get(string command, string commandArgs)
		{
			var ex = new ConsoleAppUnableToGetExitCodeException("unable to get exit code after running console application");
			ex.Data["command"] = command;
			ex.Data["command-args"] = commandArgs;
			return ex;
		}
	}
}