using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Repo;

namespace Drone.Lib.RequestModules.FileModule
{
	public class FileRequestParser : IRequestParser
	{
		public object GetParameter(IList<string> args)
		{
			if(args.Count == 0)
				throw new Exception("too few arguments!");

			var result = null as object;

			var command = args.FirstOrDefault();
			args.RemoveAt(0);

			var projectFilename = Dronefile.DefaultFilename;

			if (command == "new")
			{
				result = this.GetNewParameter(args, projectFilename);
			}
			else if (command.EndsWith(".zm"))
			{
				if (args.Count > 0)
				{
					projectFilename = command;
					command = args.FirstOrDefault();
					args.RemoveAt(0);	
				}
				else
				{
					throw new Exception("expected more parameters after project filename");
				}
			}
			
			if (command == "add")
			{
				result = this.GetAddParameter(args);
			}
			else if (command == "rm")
			{
				result = this.GetRemoveParameter(args);
			}

			if(result == null)
			{
				throw new Exception("unable to determine command for project");
			}

			return result;
		}

		private object GetAddParameter(IList<string> args)
		{
			if (args.Count == 0)
			{
				throw new Exception("expected at least 1 file when adding to a project");
			}

			return new AddParameter(args);
		}

		private object GetRemoveParameter(IList<string> args)
		{
			if (args.Count == 0)
			{
				throw new Exception("expected at least 1 file when adding to a project");
			}

			return new RemoveParameter(args);
		}

		private object GetNewParameter(IList<string> args, string defaultProjectFilename)
		{
			var projectFilename = defaultProjectFilename;

			if (args.Count > 0)
			{
				projectFilename = args.FirstOrDefault();
			}

			return new NewParameter();
		}
	}
}