using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Core
{
	[Serializable]
	public class DroneCreateMainModuleFailedException : Exception
	{
		public DroneCreateMainModuleFailedException()
		{
		}

		public DroneCreateMainModuleFailedException(string message) : base(message)
		{
		}

		public DroneCreateMainModuleFailedException(string message, Exception inner) : base(message, inner)
		{
		}

		protected DroneCreateMainModuleFailedException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static DroneCreateMainModuleFailedException Get(Exception ex, Type type)
		{
			var msg = "unable to create main module from type";
			var nex = new DroneCreateMainModuleFailedException(msg, ex);

			nex.Data["type"] = type.FullName;
			
			return nex;
		}
	}
}