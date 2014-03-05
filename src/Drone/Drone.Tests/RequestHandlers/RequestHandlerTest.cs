using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.RequestHandlers;

namespace Drone.Tests.RequestHandlers
{
	public class RequestHandlerTest : TestBase
	{
		private RequestRunner runner;

		public override void FixtureSetUp()
		{
			this.runner = new RequestRunner();
		}

		public void Run(string request)
		{
			this.runner.Run(request);
		}
	}
}