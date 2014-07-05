using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Drone.Lib.Tasks
{
	public delegate void RunProcessTaskOutputHandler(string line, bool isError);

	public delegate void RunProcessTaskResultHandler(RunProcessTaskResult result);

	public class RunProcessTask : DroneTask
	{
		public string Filename { get; set; }

		public string Args { get; set; }

		public string WorkDir { get; set; }

		public LogLevel OutputLogLevel { get; set; }

		public RunProcessTaskOutputHandler OnOutputReceived { get; set; }

		public RunProcessTaskResultHandler OnFinished { get; set; }

		public RunProcessTask()
		{
			this.OutputLogLevel = LogLevel.Info;
		}

		public override DroneTask Clone(string newName)
		{
			return this.Clone(newName, x =>
			{
				x.Filename = this.Filename;
				x.Args = this.Args;
				x.WorkDir = this.WorkDir;
			});
		}
	}
}
