using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Core;

namespace Drone.Lib.Helpers
{
	public class StringToken
	{
		public int Index { get; set; }

		public string Value { get; set; }

		public StringTokenType Type { get; set; }

		public StringToken(int index, string val, StringTokenType type)
		{
			if(string.IsNullOrWhiteSpace(val))
				throw new ArgumentException("val is empty or null", "val");

			this.Index = index;
			this.Value = val;
			this.Type = type;
		}
	}
}