using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public class HumanFriendlyTime
	{
		public static string Get(TimeSpan ts)
		{
			if (ts.Days > 0)
			{
				return GetCore(ts.Days, "d", ts.Hours, "h");
			}
			else
			{
				if (ts.Hours > 0)
				{
					return GetCore(ts.Hours, "h", ts.Minutes, "m");
				}
				else
				{
					if (ts.Minutes > 0)
					{
						return GetCore(ts.Minutes, "m", ts.Seconds, "s");
					}
					else
					{
						if (ts.Seconds > 0)
						{
							return GetCore(ts.Seconds, "s", ts.Milliseconds, "ms");
						}
						else
						{
							return GetCore(ts.Milliseconds, "ms", 0, string.Empty);
						}
					}
				}
			}
		}

		private static string GetCore(int whole, string wholeUnit, int part, string partUnit)
		{
			if(part == 0)
			{
				return string.Format("{0}{1}", whole, wholeUnit);
			}
			else
			{
				return string.Format("{0}{1} {2}{3}", whole, wholeUnit, part, partUnit);
			}
		}
	}
}