using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Drone.Lib.Exceptions
{
	[Serializable]
	public class RequestHandlerNotFoundException : Exception
	{
		public RequestHandlerNotFoundException()
		{
		}

		public RequestHandlerNotFoundException(string message) : base(message)
		{
		}

		public RequestHandlerNotFoundException(string message, Exception inner) : base(message, inner)
		{
		}

		protected RequestHandlerNotFoundException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static RequestHandlerNotFoundException Get(object parameter)
		{
			var ex = new RequestHandlerNotFoundException("Unable to get handler for parameter");

			ex.Data["parameter"] = parameter != null ? parameter.ToString() : string.Empty;
			
			return ex;
		}
	}
}