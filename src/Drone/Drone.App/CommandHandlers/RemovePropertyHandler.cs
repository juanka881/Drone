using System;
using System.Collections.Generic;
using System.Linq;
using Drone.App.Core;
using Drone.Lib.Core;
using Drone.Lib.Helpers;

namespace Drone.App.CommandHandlers
{
	public class RemovePropertyHandler : CommandHandler
	{
		public override void Handle(StringTokenSet tokens)
		{
			var config = this.Config;

			var key = tokens.TryGetAt(0);

			if (key != null)
			{
				config.Properties.Remove(key.Value);
				this.SaveConfig(config);

				this.Log.Info("property '{0}' removed", key.Value);
			}
		}
	}
}