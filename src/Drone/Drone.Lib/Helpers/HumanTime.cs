using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public class HumanTime
	{
		public const int TicksPerMicrosecond = 10;

		public const int NanosecondsPerTick = 100;

		public static string Format(TimeSpan ts)
		{
			if (ts.Days > 0)
			{
				return FormatCore(ts.Days, "d", ts.Hours, "h");
			}
			else
			{
				if (ts.Hours > 0)
				{
					return FormatCore(ts.Hours, "h", ts.Minutes, "mn");
				}
				else
				{
					if (ts.Minutes > 0)
					{
						return FormatCore(ts.Minutes, "mn", ts.Seconds, "s");
					}
					else
					{
						if (ts.Seconds > 0)
						{
							return FormatCore(ts.Seconds, "s", ts.Milliseconds, "ms");
						}
						else
						{
							if (ts.Milliseconds > 0)
							{
								return FormatCore(ts.Milliseconds, "ms", 0, string.Empty);
							}
							else
							{
								var microSecs = (int)Math.Floor((ts.Ticks % TimeSpan.TicksPerMillisecond) / (double)TicksPerMicrosecond);
								return string.Format("{0}µs", microSecs);
							}
						}
					}
				}
			}
		}

		private static string FormatCore(double whole, string wholeUnit, int part, string partUnit)
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