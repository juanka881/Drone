using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Drone.Lib.Helpers;
using Drone.Lib.Configs;

namespace Drone.Tests
{
	public class Test1 : TestBase
	{
		[Test]
		public void Test2()
		{
			var config = new DroneConfig();
			config.SetConfigFilename("drone.config");

			//config.ReferenceFiles.Register(new DroneReferenceFile(DroneReferenceFileType.File, "file1.cs"));
			//config.ReferenceFiles.Register(new DroneReferenceFile(DroneReferenceFileType.GlobalAssemblyCache, "System.Data"));

			var repo = new DroneConfigRepo();

			repo.Save(config);
		}

		[Test]
		public void Toad()
		{
			var repo = new DroneConfigRepo();

			var config = repo.Load("drone.config");

			Console.WriteLine(config);
		}
	}
}
