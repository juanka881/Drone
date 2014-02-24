using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Drone.Lib;
using Drone.Lib.Engine;
using Drone.Lib.RequestModules;

namespace Drone.App
{
	public class Program
	{
		static void Main(string[] args)
		{
			if (args.Any(x => !string.IsNullOrEmpty(x) && x.Contains("-d")))
			{
				while (!Debugger.IsAttached) { }
			}

			var runner = new DroneRequestRunner();
			runner.Run(Environment.CommandLine);
		}
	}

	public class DbModule : DroneModule
	{
		public DbModule()
		{
			this.Register("c", c => { }); //c.Log.Info("compiling"));
		}
	}

	public class Dronemain : DroneModule
	{
		public Dronemain()
		{
			this.Register("main", c =>
			{
				//c.Log.Info("hello from c task!");
				Foo();
			});

			this.Register("db", new DbModule());
		}

		public void Foo()
		{
			this.Bar();
		}

		public void Bar()
		{
			try
			{
				ZZZ(null);
			}
			catch (Exception ex)
			{
				var nex = new InvalidOperationException("oh no you can't do that", ex);

				nex.Data["prop1"] = "v1";
				nex.Data["prop2"] = "1, 2, 3";
				nex.Data["prop3"] = "#foo";

				throw nex;
			}
		}

		public void ZZZ(string s)
		{
			if (s == null)
				throw new ArgumentNullException("s");

			
		}
	}
}
