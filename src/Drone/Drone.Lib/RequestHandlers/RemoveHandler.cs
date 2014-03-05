using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Core;

namespace Drone.Lib.RequestHandlers
{
	public class RemoveHandler : RequestHandler
	{
		public override void Handle(RequestTokens tokens)
		{
			//var sourceFiles = p.Files.Where(x => x.EndsWith(".cs")).ToList();
			//var referenceFiles = p.Files.Where(x => x.EndsWith(".dll")).ToList();

			//var dronefile = this.Repo.Load(p.DroneConfigFilename);

			//foreach (var sourceFile in sourceFiles)
			//	dronefile.SourceFiles.Remove(sourceFile);

			//foreach (var referenceFile in referenceFiles)
			//	dronefile.ReferenceFiles.Remove(referenceFile);

			//this.Repo.Save(dronefile, p.DroneConfigFilename);
		}
	}
}