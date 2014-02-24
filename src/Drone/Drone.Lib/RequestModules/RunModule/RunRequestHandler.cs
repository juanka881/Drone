using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Compilers;
using Drone.Lib.Repo;

namespace Drone.Lib.RequestModules.RunModule
{
	public class RunRequestHandler : RequestHandler<RunParameter>
	{
		public override void HandleCore(RunParameter parameter)
		{
			this.Log.Info("running");
			this.Log.Info("project file: \t{0}", parameter.DroneFilename);
			this.Log.Info("targets:");

			foreach (var target in parameter.TaskNames)
			{
				this.Log.Info("target: {0}", target);
			}

			this.Log.Info("parameters:");
			this.Log.Info(parameter.Parameters);

			var dronefile = this.Repo.Load(parameter.DroneFilename);

			var compiler = new DronefileCompiler();
			
			compiler.Compile(dronefile);

			this.Log.Info("all targets completed");
		}
	}
}