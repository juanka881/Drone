using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public static class OptionExtensions
	{
		public static Option<T> ToOption<T>(this T val)
		{
			return new Option<T>(val);
		}

		public static Option<R> SelectMany<T, R>(this Option<T> opt, Func<T, Option<R>> selector)
		{
			return selector(opt.Value);
		}

		public static Option<V> SelectMany<T, R, V>(this Option<T> opt, Func<T, Option<R>> optSelector, Func<T, R, V> resultSelector)
		{
			return opt.SelectMany(x => optSelector(x).SelectMany(y => resultSelector(x, y).ToOption()));
		}
	}
}