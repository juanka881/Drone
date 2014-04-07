using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Drone.App.Core;

namespace Drone.App
{
	public class Program
	{
		static void Main(string[] args)
		{
			if (args.Any(x => x == "-d"))
			{
				while (!Debugger.IsAttached) { }
			}

			var runner = new CommandRunner();
			runner.Run(Environment.CommandLine);
		}
	}
}
