using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Drone.Tests.RequestHandlers
{
	[TestFixture]
	public class InitHandlerTest : RequestHandlerTest
	{
		[Test]
		public void handle_init()
		{
			var fn = "drone.config";

			this.TestCleanUpFile(fn);

			this.Run("init");

			Assert.IsTrue(File.Exists(fn));
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
