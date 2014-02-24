using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public static class DictionaryExtensions
	{
		public static V GetOrDef<K, V>(this IDictionary<K, V> dict, K key, V def = default(V))
		{
			if (dict == null)
				throw new ArgumentNullException("dict");

			var val = def;

			if (dict.TryGetValue(key, out val))
				return val;
			else
				return def;
		}
	}
}