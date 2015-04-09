using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Drone.Lib.Helpers;
using Drone.Lib.Compilers;

namespace Drone.Lib.Tasks
{
	public class MSBuildTaskHandler : DroneTaskHandler<MSBuildTask>
	{
		private bool hasError;
		private Logger log;

		public override void Handle(MSBuildTask task, DroneTaskContext context)
		{
			var sb = new StringBuilder();

			sb.Append(task.File).Append(" ");

			if(task.Targets != null && task.Targets.Count > 0)
			{
				var targets = string.Join(";", task.Targets);
				sb.Append("/t:").Append(targets).Append(" ");
			}

			sb.Append("/v:").Append(task.Verbosity.ToString().ToLower()).Append(" ");

			if(task.NoLogo)
				sb.Append("/nologo").Append(" ");

			foreach(var item in task.Properties)
				sb.Append("/p:").Append(item.Key).Append("=").Append(item.Value).Append(" ");

			var msbuildCommand = DotNet.DotNetFramework.Version40.MSBuildBinFilePath;
			var msbuildArgs = sb.ToString();

			context.Log.Debug("command line: '{0}'", msbuildCommand);
			context.Log.Debug("command args: '{0}'", msbuildArgs);

			try
			{
				this.log = context.Log;

				using(var processRunner = new ProcessRunner(msbuildCommand, msbuildArgs))
				{
					processRunner.OutputRecevied += ProcessRunner_OnProcessOutputRecevied;
					processRunner.Start();
					var result = processRunner.WaitForExit();

					if (this.hasError)
						throw new Exception("msbuild failed");

					if (result.ExitCode > 0)
					{
						var ex = new Exception("msbuild failed: exited with a non-zero exit code");
						ex.Data["exit-code"] = result.ExitCode;
					}
				}
			}
			finally
			{
				this.log = null;
			}
		}

		private void ProcessRunner_OnProcessOutputRecevied(object sender, ProcessRunnerOutputEventArgs e)
		{
			if (this.log == null)
				return;

			if (string.IsNullOrWhiteSpace(e.Data))
				return;

			if(e.IsError)
			{
				this.hasError = true;
				this.log.Error(e.Data);
			}
			else
			{
				var level = MSBuildHelper.GetOutputLevel(e.Data);

				switch(level)
				{
					case MSBuildOutputLevel.Normal:
						this.log.Info(e.Data);
						break;

					case MSBuildOutputLevel.Error:
						this.log.Error(e.Data);
						this.hasError = true;
						break;

					case MSBuildOutputLevel.Warning:
						this.log.Warn(e.Data);
						break;
				}
			}
		}
	}
}