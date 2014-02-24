using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public static class ObjectExtensions
	{
		public static T CastTo<T>(this object obj)
		{
			return (T)obj;
		}
	}
}