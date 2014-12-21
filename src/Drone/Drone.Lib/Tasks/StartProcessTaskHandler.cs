using Drone.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Tasks
{
	public class StartProcessTaskHandler : DroneTaskHandler<StartProcessTask>
	{
		public override void Handle(StartProcessTask task, DroneTaskContext context)
		{
			var processRunner = new ProcessRunner(task.FileName, task.Args, task.WorkDir);
			processRunner.Start();
		}
	}
}