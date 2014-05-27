using System;
using System.Collections.Generic;
using System.Linq;
using Drone.App.Core;
using Drone.Lib.Core;
using Drone.Lib.Helpers;

namespace Drone.App.CommandHandlers
{
	public class HelpHandler : CommandHandler
	{
		public override void Handle(StringTokenSet tokens)
		{
			this.Log.Info("!help goes here");
		}
	}
}