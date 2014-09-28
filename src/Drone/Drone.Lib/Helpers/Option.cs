using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Helpers
{
	public struct Option<T>
	{
		public static readonly Option<T> None = new Option<T>();

		public static implicit operator Option<T>(T val)
		{
			return Option.From(val);
		}

		private readonly T value;
		private readonly bool hasValue;

		public Option(T value)
		{
			this.value = value;
			this.hasValue = true;
		}

		public bool HasValue
		{
			get
			{
				return this.hasValue;
			}
		}

		public T Value
		{
			get
			{
				if(this.hasValue)
					return this.value;

				throw OptionMissingValueException.Get(this.GetType());
			}
		}

		public R Get<R>(Func<T, R> selector, R def)
		{
			return this.hasValue ? selector(this.value) : def;
		}
	}

	public class Option
	{
		public static Option<T> None<T>()
		{
			return Option<T>.None;
		}

		public static Option<T> From<T>(T val)
		{
			if(typeof(T).IsValueType)
			{
				return new Option<T>(val);	
			}
			else
			{
				if (EqualityComparer<T>.Default.Equals(val, default(T)))
				{
					return None<T>();
				}
				else
				{
					return new Option<T>(val);
				}
			}
		}
	}
}
