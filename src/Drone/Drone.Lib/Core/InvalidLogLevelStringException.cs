using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Core
{
	[Serializable]
	public class InvalidLogLevelStringException : Exception
	{
		public InvalidLogLevelStringException()
		{
		}

		public InvalidLogLevelStringException(string message) : base(message)
		{
		}

		public InvalidLogLevelStringException(string message, Exception inner) : base(message, inner)
		{
		}

		protected InvalidLogLevelStringException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static InvalidLogLevelStringException Get(string str)
		{
			var msg = "invalid log level string. expected any of the following: off, fatal, error|err, warn, info, debug, trace";

			var ex = new InvalidLogLevelStringException(msg);

			ex.Data["is-string-null-or-empty"] = string.IsNullOrWhiteSpace(str);
			ex.Data["string"] = str;

			return ex;
		}
	}
}