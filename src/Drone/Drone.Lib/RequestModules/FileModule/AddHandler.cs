using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Helpers;
using Drone.Lib.Repo;

namespace Drone.Lib.RequestModules.FileModule
{
	public class AddHandler : RequestHandler<AddParameter>
	{
		public override void HandleCore(AddParameter parameter)
		{
			var sourceFiles = parameter.Files
				.Where(x => x.EndsWith(".cs"))
				.ToList();

			var referenceFiles = parameter.Files
				.Where(x => x.EndsWith(".dll"))
				.ToList();

			var dronefile = this.Repo.Load(parameter.DroneFilename);

			var sourceFilesToAdd = sourceFiles.Except(dronefile.SourceFiles);
			var referenceFilesToAdd = referenceFiles.Except(dronefile.ReferenceFiles);

			dronefile.SourceFiles.AddMany(sourceFilesToAdd);
			dronefile.ReferenceFiles.AddMany(referenceFilesToAdd);

			this.Repo.Save(dronefile, dronefile.FilePath);
		}
	}
}
