using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Helpers;
using NLog;
using System.Diagnostics;

namespace Drone.Lib.Core
{
	/// <summary>
	/// Represents the core of the drone app, this task runner is responsible
	/// for executing tasks from a given module.
	/// </summary>
	public class DroneTaskRunner
	{
		private readonly DroneTaskHandlerFactory factory;
		private readonly Logger log;

		/// <summary>
		/// Initializes a new instance of the <see cref="DroneTaskRunner"/> class.
		/// </summary>
		/// <param name="factory">The factory used to create task handlers to execute each task.</param>
		/// <exception cref="System.ArgumentNullException">factory</exception>
		public DroneTaskRunner(DroneTaskHandlerFactory factory)
		{
			if(factory == null)
				throw new ArgumentNullException("factory");

			this.factory = factory;
			this.log = DroneLogManager.GetLog();
		}

		/// <summary>
		/// Runs the tasks from the given module
		/// </summary>
		/// <param name="module">The module.</param>
		/// <param name="taskNames">The task names.</param>
		/// <param name="config">The configuration to use when executing the tasks.</param>
		/// <param name="flags">The flags.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">module
		/// or
		/// taskNames
		/// or
		/// config</exception>
		public IList<DroneTaskResult> Run(DroneModule module, IEnumerable<string> taskNames, DroneConfig config, DroneFlags flags)
		{
			if (module == null)
				throw new ArgumentNullException("module");

			if (taskNames == null)
				throw new ArgumentNullException("taskNames");

			if(config == null)
				throw new ArgumentNullException("config");

			if(flags == null)
				throw new ArgumentNullException("flags");

			var names = taskNames.ToList();
			var results = new List<DroneTaskResult>();
			var sw = new Stopwatch();
			sw.Start();

			if (names.Count == 0)
			{
				this.log.Debug("no task names provided, trying to run default task");

				var result = this.TryRunDefaultTask(module, config, flags);

				if(result.HasValue)
					results.Add(result.Value);
			}
			else
			{
				this.log.Debug("checking task names exist...");

				this.EnsureTaskNamesExists(module, names);

				this.log.Debug("all task names found");

				var taskFaulted = false;

				this.log.Debug("running tasks...");

				foreach(var taskName in names)
				{
					this.log.Debug("finding task '{0}'", taskName);

					var task = module.TryGet(taskName);

					if(taskFaulted)
					{
						this.log.Debug("skipping task '{0}' due to previous task failure", taskName);
						results.Add(new DroneTaskResult(task, DroneTaskState.NotRan, TimeSpan.Zero, Option.None<Exception>()));
						continue;
					}

					this.log.Debug("task '{0}' found!, running...", taskName);

					var result = this.Run(module, task.Value, config, flags, true);

					results.Add(result);

					if (!result.IsSuccess)
					{
						this.log.Debug("task '{0}' has failed, skipping all other tasks", taskName);

						taskFaulted = true;
					}

					this.log.Info(string.Empty);
				}

				this.log.Debug("all tasks completed");
			}

			if(results.Count == 0)
				return results;

			var maxNameLen = results.Max(x => x.Task.Get(t => t.Name.Length, 0));

			var totalTime = TimeSpan.FromMilliseconds(results.Select(x => x.TimeElapsed.TotalMilliseconds).Sum()); 
			
			var summaryTitle = string.Format("tasks: ({0})", HumanTime.Format(totalTime));

			this.log.Info(summaryTitle);
			this.log.Info(string.Join(string.Empty, Enumerable.Repeat("-", summaryTitle.Length)));

			foreach(var result in results)
			{
				var glyph = this.GetTaskStateGlyph(result.State);
				var state = this.GetTaskStateDesc(result.State);
				var name = result.Task.Get(x => x.Name, "[null]");
				var time = string.Format("({0})", this.GetTaskStateFormatedTime(result.State, result.TimeElapsed));
				var fmt = string.Format("{{0}} {{1, -{0}}} {{2, -10}} {{3, -10}}", maxNameLen);
				this.log.Info(fmt, glyph, name, state, time);
			}

			sw.Stop();
			this.log.Info(string.Empty);
			this.log.Info("total time: ({0})", HumanTime.Format(sw.Elapsed));

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
						.Where(x => !module.TryGet(x).HasValue)
						.Distinct()
						.ToList();

			if (tasksNotFound.Count > 0)
				throw DroneTasksNotFoundException.Get(tasksNotFound);
		}

		private Option<DroneTaskResult> TryRunDefaultTask(DroneModule module, DroneConfig config, DroneFlags flags)
		{
			var task = module.TryGet(DroneModule.DefaultTaskName);

			if(task.HasValue)
			{
				this.log.Debug("default task found, running...");
				return Option.From(this.Run(module, task.Value, config, flags, true));
			}
			else
			{
				this.log.Warn("no default task found");
				return Option.None<DroneTaskResult>();
			}
		}

		private DroneTaskResult Run(DroneModule module, DroneTask task, DroneConfig config, DroneFlags flags, bool logErrors)
		{
			var taskLog = DroneLogManager.GetTaskLog(task.Name);

			var taskContext = new DroneTaskContext(module, task, config, flags, taskLog, (t, c, f) =>
			{
				var childTaskResult = this.Run(module, t, c, f, false);

				if(!childTaskResult.IsSuccess)
				{
					this.log.Debug("child task '{0}' has failed", t.Name);
					throw childTaskResult.Exception.Value;
				}
			});

			this.log.Info("running '{0}'", task.Name);

			var handler = this.factory.TryGetHandler(task);

			var result = FuncStopwatch.Run(() =>
			{
				if(handler.HasValue)
				{
					handler.Value.Handle(taskContext);
				}
				else
				{
					throw DroneTaskHandlerNotFoundException.Get(task);
				}
			});

			if (result.IsSuccess)
			{
				this.log.Info("task '{0}' completed ({1})", task.Name, HumanTime.Format(result.TimeElapsed));
				return new DroneTaskResult(Option.From(task), DroneTaskState.Completed, result.TimeElapsed, Option.None<Exception>());
			}
			else
			{
				var state = DroneTaskState.Faulted;
				var stateName = "failed";

				this.log.Info("task '{0}' {1} ({2})", task.Name, stateName, HumanTime.Format(result.TimeElapsed));

				if(logErrors)
					this.log.ExceptionAndData(result.Exception);

				return new DroneTaskResult(Option.From(task), state, result.TimeElapsed, Option.From(result.Exception));
			}
		}
	}
}
