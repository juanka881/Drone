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
				Console.WriteLine("waiting for debugger...");
				var sw = new Stopwatch();
				sw.Start();
				while(!Debugger.IsAttached)
				{
					if(sw.Elapsed > TimeSpan.FromSeconds(30))
					{
						Console.WriteLine("waiting for debugger timeout");
						return;
					}
				}
			}

			var runner = new CommandRunner();
			runner.Run(Environment.CommandLine);
		}
	}
}
