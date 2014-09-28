using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Helpers
{
	[Serializable]
	public class OptionMissingValueException : Exception
	{
		public OptionMissingValueException()
		{
		}

		public OptionMissingValueException(string message) : base(message)
		{
		}

		public OptionMissingValueException(string message, Exception inner) : base(message, inner)
		{
		}

		protected OptionMissingValueException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}

		public static OptionMissingValueException Get(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			var ex = new OptionMissingValueException("attempted to access missing value from option");
			ex.Data["option-type"] = type.ToString();
			return ex;
		}
	}
}