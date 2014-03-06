using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Core;
using Drone.Lib.Helpers;

namespace Drone.Lib.CommandHandlers
{
	public class AddHandler : CommandHandler
	{
		public override void Handle(CommandTokens tokens)
		{
			var sourceFiles = tokens
				.Where(x => x.Value.ToLower().EndsWith(".cs"))
				.Select(x => x.Value)
				.ToList();

			var referenceFiles = tokens
				.Where(x => x.Value.ToLower().EndsWith(".dll"))
				.Select(x => x.Value)
				.ToList();

			if(sourceFiles.Count == 0 && referenceFiles.Count == 0)
			{
				this.Log.Warn("nothing to add. no files specified");
				return;
			}

			var config = this.LoadConfig();

			var sourceFilesToAdd = sourceFiles.Except(config.SourceFiles).ToList();
			var referenceFilesToAdd = referenceFiles.Except(config.ReferenceFiles).ToList();

			if (sourceFilesToAdd.Count == 0 && referenceFilesToAdd.Count == 0)
			{
				this.Log.Warn("nothing to add. files are already in config");
				return;
			}

			config.SourceFiles.AddMany(sourceFilesToAdd);
			config.ReferenceFiles.AddMany(referenceFilesToAdd);

			this.SaveConfig(config);

			foreach (var file in sourceFilesToAdd.Concat(referenceFilesToAdd))
				this.Log.Info("added '{0}'", file);
		}
	}
}
