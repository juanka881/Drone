using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Drone.Lib.RequestModules.RunModule
{
	public class RunParameter : RequestParameter
	{
		public IList<string> TaskNames { get; private set; }
		public JObject Parameters { get; private set; }

		public RunParameter(IEnumerable<string> taskNames, JObject parameters)
		{
			if (taskNames == null)
				throw new ArgumentNullException("taskNames");

			if (parameters == null)
				throw new ArgumentNullException("parameters");

			this.TaskNames = new List<string>(taskNames);
			this.Parameters = parameters;
		}
	}
}