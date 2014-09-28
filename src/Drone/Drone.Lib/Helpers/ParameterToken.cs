using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public class ParameterToken
	{
		public int Index { get; set; }

		public string Value { get; set; }

		public ParameterTokenType Type { get; set; }

		public ParameterToken(int index, string val, ParameterTokenType type)
		{
			if(string.IsNullOrWhiteSpace(val))
				throw new ArgumentException("val is empty or null", "val");

			this.Index = index;
			this.Value = val;
			this.Type = type;
		}
	}
}