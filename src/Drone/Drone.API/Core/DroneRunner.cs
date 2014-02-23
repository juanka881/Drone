using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Drone.API.Helpers;
using NLog;
using System.IO;

namespace Drone.API.Core
{
	public class DroneRunner
	{
		private void CheckTaskNames(DroneModule module, IList<string> taskNames)
		{
			var tasksNotFound = taskNames
						.Where(x => module.TryGetTask(x) == null)
						.Distinct()
						.ToList();

			if (tasksNotFound.Count > 0)
				throw DroneTasksNotFoundException.Get(tasksNotFound);
		}

		public DroneRunnerResult Run(DroneModule module, IEnumerable<string> taskNames)
		{
			if (module == null)
				throw new ArgumentNullException("module");

			if (taskNames == null)
				throw new ArgumentNullException("taskNames");

			var log = LogManager.GetLogger("drone");
			
			var sw = new Stopwatch();

			try
			{
				sw.Start();

				var names = taskNames.ToList();

				if (names.Count == 0)
				{
					log.Info(string.Empty);
					this.RunDefaultTaskIfAny(module, log);
				}
				else
				{
					this.CheckTaskNames(module, names);

					log.Info(string.Empty);

					foreach (var taskName in names)
					{
						var task = module.TryGetTask(taskName);
						this.Run(task, log);
						log.Info(string.Empty);
					}

					log.Info("total time: {0}", HumanFriendlyTime.Get(sw.Elapsed));
				}

				sw.Stop();

				return DroneRunnerResult.Success;
			}
			catch (Exception ex)
			{
				log.Error(string.Empty);
				log.Error("DRONE ERROR");

				var errors = ex.ToList();

				var counter = 1;

				foreach (var err in errors)
				{
					log.Info(string.Empty);

					log.Error("({0}/{1}) {2}", counter, errors.Count, err.Message);

					foreach (var key in err.Data.Keys)
					{
						log.Error("{0}: {1}", key, ex.Data[key]);
					}

					counter += 1;
				}

				File.WriteAllText("drone-error.txt", ex.ToString());

				log.Info(string.Empty);
				log.Info("total time: {0}", HumanFriendlyTime.Get(sw.Elapsed));

				return new DroneRunnerResult(ex);
			}
		}

		private void RunDefaultTaskIfAny(DroneModule module, Logger log)
		{
			var task = module.TryGetTask(DroneModule.DefaultTaskName);

			if (task != null)
			{
				this.Run(task, log);
			}
			else
			{
				log.Info("no default task found");
			}
		}

		private void Run(DroneTask task, Logger log)
		{
			var sw = new Stopwatch();

			try
			{
				var taskLog = LogManager.GetLogger(string.Format("drone.task.{0}", task.Name));
				var context = new DroneContext(task.Name, taskLog);

				log.Info("running '{0}'", task.Name);

				sw.Start();

				task.Action(context);

				sw.Stop();

				log.Info("FINISHED '{0}' {1}", task.Name, HumanFriendlyTime.Get(sw.Elapsed));	
			}
			catch (Exception)
			{
				sw.Stop();

				log.Error("FAILED '{0}' {1}", task.Name, HumanFriendlyTime.Get(sw.Elapsed));
				throw;
			}
		}

	}
}
