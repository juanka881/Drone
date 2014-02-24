using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Drone.Lib.Exceptions
{
	[Serializable]
	public class RequestHandlerParameterCastException : Exception
	{
		public RequestHandlerParameterCastException()
		{
		}

		public RequestHandlerParameterCastException(string message) : base(message)
		{
		}

		public RequestHandlerParameterCastException(string message, Exception inner) : base(message, inner)
		{
		}

		protected RequestHandlerParameterCastException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static RequestHandlerParameterCastException Get(object parameter, Type parameterType)
		{
			var ex = new RequestHandlerParameterCastException("Unable to cast handler parameter object to type");

			ex.Data["parameter-type"] = parameter == null ? parameter.GetType().ToString() : string.Empty;
			ex.Data["expected-parameter-type"] = parameterType.ToString();

			return ex;
		}
	}
}