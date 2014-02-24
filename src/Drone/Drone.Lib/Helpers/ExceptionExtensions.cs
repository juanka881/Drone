using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public static class ExceptionExtensions
	{
		public static IList<Exception> ToList(this Exception ex)
		{
			var list = new List<Exception>();

			for (var cex = ex; cex != null; cex = cex.InnerException)
				list.Add(cex);

			list.Reverse();
			return list;
		}
	}
}