using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Exceptions
{
	[Serializable]
	public class TooManyMainModulesFoundException : Exception
	{
		public TooManyMainModulesFoundException()
		{
		}

		public TooManyMainModulesFoundException(string message)
			: base(message)
		{
		}

		public TooManyMainModulesFoundException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected TooManyMainModulesFoundException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}

		public static TooManyMainModulesFoundException Get(IList<Type> moduleTypes)
		{
			if (moduleTypes == null)
				throw new ArgumentNullException("moduleTypes");

			var ex = new TooManyMainModulesFoundException("Too many main modules found in dronefile");

			for (var i = 0; i < moduleTypes.Count; i++)
			{
				var key = string.Format("type-{0}", i + 1);
				ex.Data[key] = moduleTypes[i].FullName;
			}

			return ex;
		}
	}
}