using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public delegate void ProcessRunnerOutputReceivedEventHandler(object sender, ProcessRunnerOutputReceivedEventArgs e);

	public class ProcessRunnerOutputReceivedEventArgs
	{
		public string Data { get; private set; }

		public bool IsError { get; private set; }

		public ProcessRunnerOutputReceivedEventArgs(string data, bool isError)
		{
			this.Data = data;

			this.IsError = isError;
		}
	}
}