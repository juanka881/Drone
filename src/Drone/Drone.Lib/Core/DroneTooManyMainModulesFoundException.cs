using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Core
{
	[Serializable]
	public class DroneTooManyMainModulesFoundException : Exception
	{
		public DroneTooManyMainModulesFoundException()
		{
		}

		public DroneTooManyMainModulesFoundException(string message)
			: base(message)
		{
		}

		public DroneTooManyMainModulesFoundException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected DroneTooManyMainModulesFoundException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}

		public static DroneTooManyMainModulesFoundException Get(IList<Type> moduleTypes)
		{
			if (moduleTypes == null)
				throw new ArgumentNullException("moduleTypes");

			var ex = new DroneTooManyMainModulesFoundException("too many main drone modules found in drone user module");

			for (var i = 0; i < moduleTypes.Count; i++)
			{
				var key = string.Format("type-{0}", i + 1);
				ex.Data[key] = moduleTypes[i].FullName;
			}

			return ex;
		}
	}
}