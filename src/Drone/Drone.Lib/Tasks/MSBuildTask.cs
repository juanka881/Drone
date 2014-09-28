using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Helpers;
using Drone.Lib.VSProjects;

namespace Drone.Lib.Tasks
{
	public class MSBuildTask : DroneTask
	{
		public IDictionary<string, string> Properties { get; set; }

		public string Configuration
		{
			get
			{
				return this.Properties.Get("Configuration");
			}
			set
			{
				this.Properties["Configuration"] = value;
			}
		}

		public string File { get; set; }

		public IList<string> Targets { get; set; }

		public bool NoLogo { get; set; }

		public MSBuildVerbosity Verbosity { get; set; }

		public MSBuildTask()
		{
			this.Properties = new Dictionary<string, string>();
			this.Targets = new List<string>();
			this.NoLogo = true;
			this.Verbosity = MSBuildVerbosity.Normal;
			this.Configuration = VSProject.DebugConfigName;
		}

		public override DroneTask Clone(string newName)
		{
			return this.Clone(newName, x =>
			{
				x.Properties = new Dictionary<string, string>(this.Properties);
				x.Targets = new List<string>(this.Targets);
				x.File = this.File;
				x.NoLogo = this.NoLogo;
			});
		}
	}
}
