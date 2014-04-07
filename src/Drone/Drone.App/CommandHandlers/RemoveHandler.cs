using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.App.Core;
using Drone.Lib.Configs;
using Drone.Lib.Core;

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

			var dlls = files
				.Where(x => x.lower.EndsWith(".dll"))
				.Select(x => new DroneReferenceFile(DroneReferenceFileType.File, x.val))
				.ToList();

			var gacRefs = files
				.Where(x => x.lower.StartsWith("gac:"))
				.Select(x => new DroneReferenceFile(DroneReferenceFileType.GlobalAssemblyCache, x.val.Replace("gac:", string.Empty)))
				.ToList();

			var refs = dlls.Concat(gacRefs).ToList();

			if (sources.Count == 0 && refs.Count == 0)
			{
				this.Log.Warn("nothing to remove. no files specified");
				return;
			}

			var config = this.LoadConfig();

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

			foreach (var file in sourcesRemoved.Concat(referencesRemoved.Select(x => x.Path)))
				this.Log.Info("removed '{0}'", file);
		}
	}
}