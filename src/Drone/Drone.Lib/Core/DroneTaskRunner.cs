using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Helpers;
using NLog;
using Drone.Lib.Configs;

namespace Drone.Lib.Core
{
	public class DroneTaskRunner
	{
		public Logger Log { get; set; }

		public IList<DroneTaskResult> Run(DroneModule module, IEnumerable<string> taskNames, DroneConfig config)
		{
			if (module == null)
				throw new ArgumentNullException("module");

			if (taskNames == null)
				throw new ArgumentNullException("taskNames");

			if(config == null)
				throw new ArgumentNullException("config");

			var names = taskNames.ToList();
			var results = new List<DroneTaskResult>();

			if (names.Count == 0)
			{
				this.Log.Debug("no task names provided, trying to run default task");

				var result = this.TryRunDefaultTask(module, config);

				if(result != null)
					results.Add(result);
			}
			else
			{
				this.Log.Debug("checking task names exist...");

				this.CheckTaskNames(module, names);

				this.Log.Debug("all task names found");

				var taskFaulted = false;

				this.Log.Debug("running tasks...");

				for(var i = 0; i < names.Count; i++)
				{
					var taskName = names[i];

					this.Log.Debug("finding task '{0}'", taskName);

					var task = module.TryGetTask(taskName);

					if(taskFaulted)
					{
						this.Log.Debug("skipping task '{0}' due to previous task failure", taskName);

						results.Add(new DroneTaskResult(task, DroneTaskState.NotRan, TimeSpan.Zero));

						continue;
					}

					this.Log.Debug("task '{0}' found!, running...", taskName);

					var result = this.Run(task, config);

					results.Add(result);

					if(!result.IsSuccess)
					{
						this.Log.Debug("task '{0}' has failed, skipping all other tasks", taskName);

						taskFaulted = true;
					}

					this.Log.Info(string.Empty);
				}

				this.Log.Debug("all tasks completed");
			}

			if(results.Count == 0)
				return results;

			var maxNameLen = results.Max(x => x.Task.Name.Length);

			var totalTime = results.Select(x => x.TimeElapsed).Aggregate((x, y) => x + y); 
			
			var summaryTitle = string.Format("tasks summary: ({0})", HumanTime.Format(totalTime));

			this.Log.Info(summaryTitle);
			this.Log.Info(string.Join(string.Empty, Enumerable.Repeat("-", summaryTitle.Length)));

			foreach(var result in results)
			{
				var glyph = this.GetGlyph(result.State);
				var state = this.GetStateDesc(result.State);
				var name = result.Task.Name;
				var time = string.Format("({0})", this.GetTime(result.State, result.TimeElapsed));
				var fmt = string.Format("{{0}} {{1, -{0}}} {{2, -10}} {{3, -10}}", maxNameLen);
				this.Log.Info(fmt, glyph, name, state, time);
			}

			return results;
		}

		private string GetTime(DroneTaskState state, TimeSpan ts)
		{
			if(state == DroneTaskState.NotRan)
				return "n/a";
			else
				return HumanTime.Format(ts);
		}

		private string GetStateDesc(DroneTaskState state)
		{
			var str = string.Empty;

			switch (state)
			{
				case DroneTaskState.NotRan:
					str = "not-ran";
					break;

				case DroneTaskState.Faulted:
					str = "faulted";
					break;

				case DroneTaskState.Cancelled:
					str = "cancelled";
					break;

				case DroneTaskState.Completed:
					str = "completed";
					break;
			}

			return str;
		}

		private string GetGlyph(DroneTaskState state)
		{
			var str = string.Empty;

			switch(state)
			{
				case DroneTaskState.NotRan:
					str = "≡";
					break;

				case DroneTaskState.Faulted:
					str = "■";
					break;

				case DroneTaskState.Cancelled:
					str = "∙";
					break;

				case DroneTaskState.Completed:
					str = "√";
					break;
			}

			return str;
		}

		private void CheckTaskNames(DroneModule module, IList<string> taskNames)
		{
			var tasksNotFound = taskNames
						.Where(x => module.TryGetTask(x) == null)
						.Distinct()
						.ToList();

			if (tasksNotFound.Count > 0)
				throw DroneTasksNotFoundException.Get(tasksNotFound);
		}

		private DroneTaskResult TryRunDefaultTask(DroneModule module, DroneConfig config)
		{
			var task = module.TryGetTask(DroneModule.DefaultTaskName);

			if (task != null)
			{
				this.Log.Debug("default task found, running...");

				return this.Run(task, config);
			}
			else
			{
				this.Log.Warn("no default task found");
				return null;
			}
		}

		private DroneTaskResult Run(DroneTask task, DroneConfig config)
		{
			var taskLog = LogHelper.GetTaskLog(task.Name);
			var context = new DroneTaskContext(task.Name, taskLog, config);

			this.Log.Info("running '{0}'", task.Name);

			var result = FuncStopwatch.Run(() => task.Action(context));

			if (result.IsSuccess)
			{
				this.Log.Info("task '{0}' completed ({1})", task.Name, HumanTime.Format(result.TimeElapsed));
				return new DroneTaskResult(task, DroneTaskState.Completed, result.TimeElapsed);
			}
			else
			{
				var state = DroneTaskState.Faulted;
				var stateName = "failed";

				if (result.Exception is DroneTaskCancelException)
				{
					state = DroneTaskState.Cancelled;
					stateName = "cancelled";
				}

				this.Log.Info("task '{0}' {1} ({2})", task.Name, stateName, HumanTime.Format(result.TimeElapsed));
				this.Log.ExceptionAndData(result.Exception);
				
				return new DroneTaskResult(task, state, result.TimeElapsed, result.Exception);
			}
		}
	}
}
