using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Core;

namespace Drone.Lib.CommandHandlers
{
	public class HelpHandler : CommandHandler
	{
		public override void Handle(CommandTokens tokens)
		{
			this.Log.Info("!help goes here");
		}
	}
}