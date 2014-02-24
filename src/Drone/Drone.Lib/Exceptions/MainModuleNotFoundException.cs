using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Exceptions
{
	[Serializable]
	public class MainModuleNotFoundException : Exception
	{
		public static MainModuleNotFoundException Get()
		{
			var ex = new MainModuleNotFoundException("Unable to find main module in dronefile");
			return ex;
		}

		public MainModuleNotFoundException()
		{
		}

		public MainModuleNotFoundException(string message) : base(message)
		{
		}

		public MainModuleNotFoundException(string message, Exception inner) : base(message, inner)
		{
		}

		protected MainModuleNotFoundException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}