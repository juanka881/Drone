using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Core;

namespace Drone.Lib.CommandHandlers
{
	public class RunHandler : CommandHandler
	{
		public override void Handle(CommandTokens tokens)
		{
			//this.Log.Info("running");
			//this.Log.Info("project file: \t{0}", parameter.DroneConfigFilename);
			//this.Log.Info("targets:");

			//foreach (var target in parameter.TaskNames)
			//{
			//	this.Log.Info("target: {0}", target);
			//}

			//this.Log.Info("parameters:");
			//this.Log.Info(parameter.Parameters);

			//var dronefile = this.Repo.Load(parameter.DroneConfigFilename);

			//var compiler = new DroneProjectCompiler();

			//compiler.Compile(dronefile);

			//this.Log.Info("all targets completed");
		}

		//public object GetParameter(IList<string> args)
		//{
		//	if (args.Count == 0)
		//		throw new Exception("too few arguments");

		//	var first = args.FirstOrDefault();

		//	var projectFilename = DroneConfig.DefaultFilename;

		//	if (first.EndsWith(".zm"))
		//	{
		//		projectFilename = first;
		//		args.RemoveAt(0);
		//	}

		//	var targets = args.Where(x => !x.StartsWith("{")).ToList();
		//	var parameterString = args.FirstOrDefault(x => x.StartsWith("{")) ?? string.Empty;
		//	var parameters = new JObject();

		//	if (!string.IsNullOrWhiteSpace(parameterString))
		//		parameters = JObject.Parse(parameterString);

		//	var result = new RunParameter(targets, parameters);

		//	return result;
		//}
	}
}