using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Drone.Lib.Helpers;
using NLog;

namespace Drone.Lib.Tasks
{
	public class RunProcessTaskHandler : DroneTaskHandler<RunProcessTask>
	{
		private RunProcessTaskOutputHandler outputHandler;
		private bool errorOutputReceived;
		private Logger log;
		private LogLevel logLevel;
		
		public override void Handle(RunProcessTask task, DroneTaskContext context)
		{
			var process = null as Process;

			try
			{
				this.log = context.Log;
				this.logLevel = task.OutputLogLevel;

				if (log.IsDebugEnabled)
				{
					log.Debug("process args");
					log.Debug("filename: {0}", task.Filename);
					log.Debug("arguments: {0}", task.Args);
					log.Debug("workdir: {0}", task.WorkDir);
				}

				log.Debug("creating process runner");
				using(var processRunner = new ProcessRunner(task.Filename, task.Args, task.WorkDir, true))
				{
					log.Debug("starting process runner");
					processRunner.Start();

					
					processRunner.ProcessOutputRecevied += ProcessRunner_OnProcessOutputRecevied;

					log.Info("process started, waiting for exit");

					var result = processRunner.WaitForExit();

					log.Info("process completed");

					if (log.IsDebugEnabled)
					{
						log.Debug("process run summary:");
						log.Debug("error output received?: {0}", this.errorOutputReceived);
						log.Debug("exit code: {0}", result.ExitCode);
						log.Debug("exit timestamp: {0}", result.ExitTimestamp);
					}

					if (task.OnFinished != null)
					{
						log.Debug("invoking on-finished handler");

						task.OnFinished(new RunProcessTaskResult(this.errorOutputReceived, process.ExitCode));

						log.Debug("on-finished handler completed");
					}
				}
			}
			finally
			{
				this.outputHandler = null;
				this.log = null;
				this.logLevel = null;
			}
		}

		private void ProcessRunner_OnProcessOutputRecevied(object sender, ProcessRunnerOutputReceivedEventArgs e)
		{
			if(e.IsError)
			{
				this.errorOutputReceived = true;

				if (this.log != null)
					this.log.Error(e.Data);	
			}
			else
			{
				if (this.log != null && this.logLevel != null)
					this.log.Log(this.logLevel, e.Data);
			}

			if (this.outputHandler != null)
				this.outputHandler(e.Data, true);
		}
	}
}