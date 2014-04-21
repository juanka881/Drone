using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Core
{
	public class Option<T>
	{
		public static readonly Option<T> None = new Option<T>();

		public T Value { get; private set; }

		public bool HasValue { get; set; }

		private Option()
		{
			this.HasValue = false;
		}

		public Option(T val)
		{
			this.Value = val;
			this.HasValue = true;
		}
	}
}
