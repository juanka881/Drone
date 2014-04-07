using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Configs;
using NUnit.Framework;

namespace Drone.Tests.CommandHandlers
{
	//public class RemoveHandlerTest : CommandHandlerTest
	//{
	//	[Test]
	//	public void handle_remove_nothing()
	//	{
	//		this.Run("init");
	//		this.Run("rm");

	//		var config = repo.Load(DroneConfig.DefaultConfigFilename);

	//		Assert.AreEqual(0, config.SourceFiles.Count);
	//		Assert.AreEqual(0, config.ReferenceFiles.Count);
	//	}

	//	[Test]
	//	public void handle_remove_sourcefiles()
	//	{
	//		this.Run("init");
	//		this.Run("add file1.cs");

	//		var config = repo.Load(DroneConfig.DefaultConfigFilename);

	//		Assert.AreEqual(1, config.SourceFiles.Count);
	//		Assert.AreEqual(0, config.ReferenceFiles.Count);

	//		this.Run("rm file1.cs");

	//		config = repo.Load(DroneConfig.DefaultConfigFilename);

	//		Assert.AreEqual(0, config.SourceFiles.Count);
	//		Assert.AreEqual(0, config.ReferenceFiles.Count);
	//	}
	//}
}