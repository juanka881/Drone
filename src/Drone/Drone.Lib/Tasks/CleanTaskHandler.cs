using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Drone.Lib.Core;

namespace Drone.Lib.Tasks
{
	public class CleanTaskHandler : DroneTaskHandler<CleanTask>
	{
		public override void Handle(CleanTask task, DroneTaskContext context)
		{
			foreach(var target in task.Targets)
			{
				try
				{
					if(Directory.Exists(target))
					{
						Directory.Delete(target, true);
					}
					else if(File.Exists(target))
					{
						File.Delete(target);
					}
				}
				catch(Exception ex)
				{
					context.Log.ExceptionAndData(ex);
				}
			}
		}
	}
}