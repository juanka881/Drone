using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Core
{
	[Serializable]
	public class DroneAssemblyLoadException : Exception
	{
		public DroneAssemblyLoadException()
		{
		}

		public DroneAssemblyLoadException(string message) : base(message)
		{
		}

		public DroneAssemblyLoadException(string message, Exception inner) : base(message, inner)
		{
		}

		protected DroneAssemblyLoadException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static DroneAssemblyLoadException Get(string assemblyFilepath, Exception sex)
		{
			var ex = new DroneAssemblyLoadException("unable to load drone user assembly", sex);
			ex.Data["assembly-filepath"] = assemblyFilepath;
			return ex;
		}
	}
}