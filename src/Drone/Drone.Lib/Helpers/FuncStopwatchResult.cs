using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public class FuncStopwatchResult
	{
		public bool IsSuccess
		{
			get
			{
				return this.Exception == null;
			}
		}

		public Exception Exception { get; private set; }

		public TimeSpan TimeElapsed { get; private set; }

		public FuncStopwatchResult(TimeSpan ts)
		{
			this.TimeElapsed = ts;
			this.Exception = null;
		}

		public FuncStopwatchResult(Exception ex, TimeSpan ts)
		{
			if (ex == null)
				throw new ArgumentNullException("ex");

			this.Exception = ex;
			this.TimeElapsed = ts;
		}
	}

	public class FuncStopwatchResult<T> : FuncStopwatchResult
	{
		public T Result { get; private set; }

		public FuncStopwatchResult(T result, TimeSpan ts)
			: base(ts)
		{
			this.Result = result;
		}

		public FuncStopwatchResult(Exception ex, TimeSpan ts)
			: base(ex, ts)
		{
			
		}
	}
}