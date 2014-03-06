using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Core;

namespace Drone.Lib.CommandHandlers
{
	public class RemoveHandler : CommandHandler
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

			if (sourceFiles.Count == 0 && referenceFiles.Count == 0)
			{
				this.Log.Warn("nothing to remove. no files specified");
				return;
			}

			var config = this.LoadConfig();

			var sourceFilesRemoved = sourceFiles
				.Where(x => config.SourceFiles.Remove(x))
				.ToList();

			var referenceFilesRemoved = referenceFiles
				.Where(x => config.ReferenceFiles.Remove(x))
				.ToList();

			if (sourceFilesRemoved.Count == 0 && referenceFilesRemoved.Count == 0)
			{
				this.Log.Warn("nothing to remove. files dont exists in config");
				return;
			}

			this.SaveConfig(config);

			foreach (var file in sourceFilesRemoved.Concat(referenceFilesRemoved))
				this.Log.Info("removed '{0}'", file);
		}
	}
}