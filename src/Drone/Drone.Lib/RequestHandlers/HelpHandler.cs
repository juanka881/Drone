using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Core;

namespace Drone.Lib.RequestHandlers
{
	public class HelpHandler : RequestHandler
	{
		public override void Handle(RequestTokens tokens)
		{
			this.Log.Info("!help goes here");
		}
	}
}