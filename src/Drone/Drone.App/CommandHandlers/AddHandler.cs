using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.App.Core;
using Drone.Lib.Helpers;

namespace Drone.App.CommandHandlers
{
	public class AddHandler : CommandHandler
	{
		public override void Handle(StringTokenSet tokens)
		{
			var files = tokens.Select(x => new 
				{
					lower = x.Value.ToLower(),
					val = x.Value
				}).ToList();

			var sources = files
				.Where(x => x.lower.EndsWith(".cs"))
				.Select(x => x.val)
				.ToList();

			var refs = files
				.Where(x => x.lower.EndsWith(".dll"))
				.Select(x => x.val)
				.ToList();

			if(sources.Count == 0 && refs.Count == 0)
			{
				this.Log.Warn("nothing to add. no files specified");
				return;
			}

			var config = this.Config;

			var sourcesToAdd = sources.Except(config.SourceFiles).ToList();

			var referencesToAdd = refs.Except(config.ReferenceFiles).ToList();

			if (sourcesToAdd.Count == 0 && referencesToAdd.Count == 0)
			{
				this.Log.Warn("nothing to add. files are already in config");
				return;
			}

			config.SourceFiles.AddMany(sourcesToAdd);
			config.ReferenceFiles.AddMany(referencesToAdd);

			this.SaveConfig(config);

			foreach (var file in sourcesToAdd.Concat(referencesToAdd))
				this.Log.Info("added '{0}'", file);
		}
	}
}
