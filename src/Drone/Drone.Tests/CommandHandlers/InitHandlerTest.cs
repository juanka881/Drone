using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Drone.Lib.Configs;
using NUnit.Framework;

namespace Drone.Tests.CommandHandlers
{
	public class InitHandlerTest : CommandHandlerTest
	{
		[Test]
		public void handle_init()
		{
			this.Run("init");

			Assert.IsTrue(File.Exists(DroneConfig.DefaultFilename));
		}

		[Test]
		public void handle_init_with_filename()
		{
			var fn = "custom.json";
			this.TestCleanUpFile(fn);

			this.Run(string.Format("init -f {0}", fn));

			Assert.IsTrue(File.Exists(fn));
		}
	}
}
