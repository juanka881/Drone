using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.CommandHandlers;
using Drone.Lib.Configs;

namespace Drone.Tests.CommandHandlers
{
	public class CommandHandlerTest : TestBase
	{
		private CommandRunner runner;
		protected DroneConfigRepo repo;

		public override void SetUp()
		{
			this.CleanUpFile(DroneConfig.DefaultFilename);
		}

		public override void TearDown()
		{
			this.CleanUpFile(DroneConfig.DefaultFilename);
		}

		public override void FixtureSetUp()
		{
			this.repo = new DroneConfigRepo();
			this.runner = new CommandRunner();
		}

		public void Run(string request)
		{
			this.runner.Run(request);
		}
	}
}