using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Repo;

namespace Drone.Lib.RequestModules.FileModule
{
	public class RemoveHandler : RequestHandler<RemoveParameter>
	{
		public override void HandleCore(RemoveParameter p)
		{
			var sourceFiles = p.Files.Where(x => x.EndsWith(".cs")).ToList();
			var referenceFiles = p.Files.Where(x => x.EndsWith(".dll")).ToList();

			var dronefile = this.Repo.Load(p.DroneFilename);

			foreach (var sourceFile in sourceFiles)
				dronefile.SourceFiles.Remove(sourceFile);

			foreach (var referenceFile in referenceFiles)
				dronefile.ReferenceFiles.Remove(referenceFile);

			this.Repo.Save(dronefile, p.DroneFilename);
		}
	}
}