using System;
using System.Collections.Generic;
using System.Linq;
using Drone.App.Core;
using Drone.Lib.Core;
using Drone.Lib.Helpers;
using Newtonsoft.Json.Linq;

namespace Drone.App.CommandHandlers
{
	public class GetPropertyHandler : CommandHandler
	{
		public override void Handle(StringTokenSet tokens)
		{
			var config = this.Config;

			var key = tokens.TryGetAt(0);

			if(key == null)
			{
				foreach(var prop in config.Properties)
				{
					this.Log.Info("{0}: {1}", prop.Key, prop.Value);
					this.Log.Info(string.Empty);
				}
			}
			else
			{
				var token = null as JToken;
				if(!config.Properties.TryGetValue(key.Value, out token))
					return;

				this.Log.Info("{0}: {1}", key.Value, token);
			}
		}
	}
}