using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Configs;
using NUnit.Framework;

namespace Drone.Tests.CommandHandlers
{
	public class AddHandlerTest : CommandHandlerTest
	{
		[Test]
		public void handle_add_nothing()
		{
			this.Run("init");
			this.Run("add");

			var config = repo.Load(DroneConfig.DefaultFilename);

			Assert.AreEqual(0, config.SourceFiles.Count);
			Assert.AreEqual(0, config.ReferenceFiles.Count);
		}

		[Test]
		public void handle_add_sourcefiles()
		{
			this.Run("init");
			this.Run("add file1.cs");

			var config = this.repo.Load(DroneConfig.DefaultFilename);

			Assert.AreEqual(1, config.SourceFiles.Count);
			Assert.AreEqual(0, config.ReferenceFiles.Count);
		}

		[Test]
		public void handle_add_same_sourcefiles()
		{
			this.Run("init");
			this.Run("add file1.cs file1.cs");

			var config = this.repo.Load(DroneConfig.DefaultFilename);

			Assert.AreEqual(1, config.SourceFiles.Count);
			Assert.AreEqual(0, config.ReferenceFiles.Count);
		}
	}
}