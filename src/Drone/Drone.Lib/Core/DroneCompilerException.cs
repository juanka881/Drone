using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Drone.Lib.Compilers;

namespace Drone.Lib.Core
{
	[Serializable]
	public class DroneCompilerException : Exception
	{
		public DroneCompilerException()
		{
		}

		public DroneCompilerException(string message) : base(message)
		{
		}

		public DroneCompilerException(string message, Exception inner) : base(message, inner)
		{
		}

		protected DroneCompilerException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static DroneCompilerException Get(CSharpCompilerResult result)
		{
			if(result.IsSuccess)
				throw new InvalidOperationException("unable to create drone compiler exception from success result");

			var ex = new DroneCompilerException("drone compiler error", result.Failure.Exception);
			return ex;
		}
	}
}