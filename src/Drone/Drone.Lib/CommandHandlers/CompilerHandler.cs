using Drone.Lib.Compilers;
using Drone.Lib.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.CommandHandlers
{
	public class CompilerHandler : CommandHandler
	{
		public override void Handle(CommandTokens tokens)
		{
			var config = this.LoadConfig();

			var project = new DroneProject(config);
			var compiler = new DroneProjectCompiler();

			compiler.Compile(project);

			this.Log.Info("compiled '{0}'", this.Flags.ConfigFilename);
		}
	}
}