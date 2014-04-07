using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Core
{
	[Serializable]
	public class DroneMainModuleNotFoundException : Exception
	{
		public DroneMainModuleNotFoundException()
		{
		}

		public DroneMainModuleNotFoundException(string message) : base(message)
		{
		}

		public DroneMainModuleNotFoundException(string message, Exception inner) : base(message, inner)
		{
		}

		protected DroneMainModuleNotFoundException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static DroneMainModuleNotFoundException Get()
		{
			var ex = new DroneMainModuleNotFoundException("unable to find main module in drone user module");
			return ex;
		}
	}
}