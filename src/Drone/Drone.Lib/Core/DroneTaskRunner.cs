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
		private readonly DroneTaskHandlerFactory factory;

		public DroneTaskRunner(DroneTaskHandlerFactory factory)
		{
			if(factory == null)
				throw new ArgumentNullException("factory");

			this.factory = factory;
		}

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

				this.EnsureTaskNamesExists(module, names);

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

					var result = this.Run(module, task, config, true);

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
				var glyph = this.GetTaskStateGlyph(result.State);
				var state = this.GetTaskStateDesc(result.State);
				var name = result.Task.Name;
				var time = string.Format("({0})", this.GetTaskStateFormatedTime(result.State, result.TimeElapsed));
				var fmt = string.Format("{{0}} {{1, -{0}}} {{2, -10}} {{3, -10}}", maxNameLen);
				this.Log.Info(fmt, glyph, name, state, time);
			}

			return results;
		}

		private string GetTaskStateFormatedTime(DroneTaskState state, TimeSpan ts)
		{
			if(state == DroneTaskState.NotRan)
				return "n/a";
			else
				return HumanTime.Format(ts);
		}

		private string GetTaskStateDesc(DroneTaskState state)
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

				case DroneTaskState.Completed:
					str = "completed";
					break;
			}

			return str;
		}

		private string GetTaskStateGlyph(DroneTaskState state)
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

				case DroneTaskState.Completed:
					str = "√";
					break;
			}

			return str;
		}

		private void EnsureTaskNamesExists(DroneModule module, IList<string> taskNames)
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

				return this.Run(module, task, config, true);
			}
			else
			{
				this.Log.Warn("no default task found");
				return null;
			}
		}

		private DroneTaskResult Run(DroneModule module, DroneTask task, DroneConfig config, bool logErrors)
		{
			var taskLog = LogHelper.GetTaskLog(task.Name);

			var context = new DroneTaskContext(module, task, config, taskLog, (t, c) =>
			{
				var childTaskResult = this.Run(module, t, c, false);

				if(!childTaskResult.IsSuccess)
					this.Log.Debug("child task '{0}' has failed", t.Name);

				throw childTaskResult.Exception;
			});

			this.Log.Info("running '{0}'", task.Name);

			var handler = this.factory.TryGetHandler(task);

			var result = FuncStopwatch.Run(() =>
			{
				if(handler == null)
					throw DroneTaskHandlerNotFoundException.Get(task);

				handler.Handle(context);
			});

			if (result.IsSuccess)
			{
				this.Log.Info("task '{0}' completed ({1})", task.Name, HumanTime.Format(result.TimeElapsed));
				return new DroneTaskResult(task, DroneTaskState.Completed, result.TimeElapsed);
			}
			else
			{
				var state = DroneTaskState.Faulted;
				var stateName = "failed";

				this.Log.Info("task '{0}' {1} ({2})", task.Name, stateName, HumanTime.Format(result.TimeElapsed));

				if(logErrors)
					this.Log.ExceptionAndData(result.Exception);
				
				return new DroneTaskResult(task, state, result.TimeElapsed, result.Exception);
			}
		}
	}
}
