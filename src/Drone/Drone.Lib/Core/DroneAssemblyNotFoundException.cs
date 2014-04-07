using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Core
{
	[Serializable]
	public class DroneAssemblyNotFoundException : Exception
	{
		public DroneAssemblyNotFoundException()
		{
		}

		public DroneAssemblyNotFoundException(string message) : base(message)
		{
		}

		public DroneAssemblyNotFoundException(string message, Exception inner) : base(message, inner)
		{
		}

		protected DroneAssemblyNotFoundException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static DroneAssemblyNotFoundException Get(string assemblyFilepath)
		{
			var ex = new DroneAssemblyNotFoundException("unable to find drone user module assembly");
			ex.Data["assembly-filepath"] = assemblyFilepath;
			return ex;
		}
	}
}