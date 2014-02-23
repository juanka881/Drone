using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.API;
using Drone.API.Core;

namespace Drone.App
{
	public class Program
	{
		static void Main(string[] args)
		{
			var runner = new DroneRunner();
			runner.Run(new Dronefile(), args);
		}
	}

	public class DbModule : DroneModule
	{
		public DbModule()
		{
			this.Register("c", c => { }); //c.Log.Info("compiling"));
		}
	}

	public class Dronefile : DroneModule
	{
		public Dronefile()
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
