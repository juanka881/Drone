using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.App.Core;
using Drone.Lib.Configs;
using Drone.Lib.Core;
using Drone.Lib.Helpers;

namespace Drone.App.CommandHandlers
{
	public class RemoveHandler : CommandHandler
	{
		public override void Handle(StringTokenSet tokens)
		{
			var files = tokens
				.Select(x => new
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

			if (sources.Count == 0 && refs.Count == 0)
			{
				this.Log.Warn("nothing to remove. no files specified");
				return;
			}

			var config = this.Config;

			var sourcesRemoved = sources
				.Where(x => config.SourceFiles.Remove(x))
				.ToList();

			var referencesRemoved = refs
				.Where(x => config.ReferenceFiles.Remove(x))
				.ToList();

			if (sourcesRemoved.Count == 0 && referencesRemoved.Count == 0)
			{
				this.Log.Warn("nothing to remove. files dont exists in config");
				return;
			}

			this.SaveConfig(config);

			foreach (var file in sourcesRemoved.Concat(referencesRemoved))
				this.Log.Info("removed '{0}'", file);
		}
	}
}