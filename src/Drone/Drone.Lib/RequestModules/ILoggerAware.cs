using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Drone.Lib.RequestModules
{
	public interface ILoggerAware
	{
		Logger Log { get; set; }
	}
}