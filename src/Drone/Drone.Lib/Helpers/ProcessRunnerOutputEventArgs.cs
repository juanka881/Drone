using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public delegate void ProcessRunnerOutputEventHandler(object sender, ProcessRunnerOutputEventArgs e);

	public class ProcessRunnerOutputEventArgs
	{
		public string Data { get; private set; }

		public bool IsError { get; private set; }

		public ProcessRunnerOutputEventArgs(string data, bool isError)
		{
			this.Data = data;

			this.IsError = isError;
		}
	}
}