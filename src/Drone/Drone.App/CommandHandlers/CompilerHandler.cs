using System;
using System.Collections.Generic;
using System.Linq;
using Drone.App.Core;
using Drone.Lib.Core;
using Drone.Lib.Helpers;
using NLog;

namespace Drone.App.CommandHandlers
{
	public class CompilerHandler : CommandHandler
	{
		public override void Handle(StringTokenSet tokens)
		{
			this.Compiler.Compile(this.Config, LogLevel.Info);
		}
	}
}