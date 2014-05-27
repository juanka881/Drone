using System;
using System.Collections.Generic;
using System.Linq;
using Drone.App.Core;
using Drone.Lib.Core;
using Drone.Lib.Helpers;
using NLog;

namespace Drone.App.CommandHandlers
{
	public class ListHandler : CommandHandler
	{
		public override void Handle(StringTokenSet tokens)
		{
			var config = this.LoadConfig();
			var module = this.CompileAndLoadModule(config, LogLevel.Debug);
			var taskCounter = 0;

			this.Log.Info("tasks:");

			foreach(var task in module.Tasks)
			{
				this.Log.Info("{0}─ {1}", this.GetPositionSymbol(taskCounter, module.TaskCount - 1), task.Name);

				if(task.Dependencies.Count > 0)
				{
					var depCounter = 0;

					foreach(var dep in task.Dependencies)
					{
						this.Log.Info("│  {0}─ {1}", this.GetPositionSymbol(depCounter, task.Dependencies.Count - 1), dep);
						depCounter += 1;
					}
				}

				taskCounter += 1;
			}
		}

		private string GetPositionSymbol(int pos, int max)
		{
			if (pos == max)
				return "└";

			return "├";
		}
	}
}