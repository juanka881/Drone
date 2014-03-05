using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Core;

namespace Drone.Lib.CommandHandlers
{
	public class AddHandler : CommandHandler
	{
		public override void Handle(CommandTokens tokens)
		{
			//var sourceFiles = parameter.Files
			//	.Where(x => x.EndsWith(".cs"))
			//	.ToList();

			//var referenceFiles = parameter.Files
			//	.Where(x => x.EndsWith(".dll"))
			//	.ToList();

			//var dronefile = this.Repo.Load(parameter.DroneConfigFilename);

			//var sourceFilesToAdd = sourceFiles.Except(dronefile.SourceFiles);
			//var referenceFilesToAdd = referenceFiles.Except(dronefile.ReferenceFiles);

			//dronefile.SourceFiles.AddMany(sourceFilesToAdd);
			//dronefile.ReferenceFiles.AddMany(referenceFilesToAdd);

			//this.Repo.Save(dronefile, dronefile.FilePath);
		}

		//public RequestParameter GetParameter(IList<string> tokens)
		//{
		//	if (tokens.Count == 0)
		//		throw new Exception("too few arguments!");

		//	var result = null as RequestParameter;

		//	var command = tokens.FirstOrDefault();
		//	tokens.RemoveAt(0);

		//	var projectFilename = DroneConfig.DefaultFilename;

		//	if (command == "new")
		//	{
		//		result = this.GetNewParameter(tokens, projectFilename);
		//	}
		//	else if (command.EndsWith(".zm"))
		//	{
		//		if (tokens.Count > 0)
		//		{
		//			projectFilename = command;
		//			command = tokens.FirstOrDefault();
		//			tokens.RemoveAt(0);
		//		}
		//		else
		//		{
		//			throw new Exception("expected more parameters after project filename");
		//		}
		//	}

		//	if (command == "add")
		//	{
		//		result = this.GetAddParameter(tokens);
		//	}
		//	else if (command == "rm")
		//	{
		//		result = this.GetRemoveParameter(tokens);
		//	}

		//	if (result == null)
		//	{
		//		throw new Exception("unable to determine command for project");
		//	}

		//	return result;
		//}

		//private RequestParameter GetAddParameter(IList<string> tokens)
		//{
		//	if (tokens.Count == 0)
		//	{
		//		throw new Exception("expected at least 1 file when adding to a project");
		//	}

		//	return new AddParameter(tokens);
		//}

		//private RequestParameter GetRemoveParameter(IList<string> tokens)
		//{
		//	if (tokens.Count == 0)
		//	{
		//		throw new Exception("expected at least 1 file when adding to a project");
		//	}

		//	return new RemoveParameter(tokens);
		//}

		//private RequestParameter GetNewParameter(IList<string> tokens, string defaultProjectFilename)
		//{
		//	var projectFilename = defaultProjectFilename;

		//	if (tokens.Count > 0)
		//		projectFilename = tokens.FirstOrDefault();

		//	return new NewParameter();
		//}
	}
}
