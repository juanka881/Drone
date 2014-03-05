using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.CommandHandlers;

namespace Drone.Tests.CommandHandlers
{
	public class CommandHandlerTest : TestBase
	{
		private CommandRunner runner;

		public override void FixtureSetUp()
		{
			this.runner = new CommandRunner();
		}

		public void Run(string request)
		{
			this.runner.Run(request);
		}
	}
}