using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Drone.Lib.Exceptions
{
	[Serializable]
	public class RequestParserNotFoundException : Exception
	{
		public RequestParserNotFoundException()
		{
		}

		public RequestParserNotFoundException(string message) : base(message)
		{
		}

		public RequestParserNotFoundException(string message, Exception inner) : base(message, inner)
		{
		}

		protected RequestParserNotFoundException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static RequestParserNotFoundException Get(string request)
		{
			var ex = new RequestParserNotFoundException("Unable to get request parser for request string");

			ex.Data["request"] = request;

			return ex;
		}
	}
}