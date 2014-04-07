using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Drone.Lib.Core
{
	public class FuncStopwatch
	{
		public static FuncStopwatchResult<T> Run<T>(Func<T> fn)
		{
			if(fn == null)
				throw new ArgumentNullException("fn");

			var sw = new Stopwatch();

			try
			{
				sw.Start();
				var result = fn();
				sw.Stop();

				return new FuncStopwatchResult<T>(result, sw.Elapsed);
			}
			catch(Exception ex)
			{
				sw.Stop();

				return new FuncStopwatchResult<T>(ex, sw.Elapsed);
			}
		}

		public static FuncStopwatchResult Run(Action fn)
		{
			if(fn == null)
				throw new ArgumentNullException("fn");

			var result = Run(() =>
			{
				fn();
				return 0;
			});

			if(result.IsSuccess)
				return new FuncStopwatchResult(result.TimeElapsed);
			else
				return new FuncStopwatchResult(result.Exception, result.TimeElapsed);
		}
	}
}