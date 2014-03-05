using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Drone.Lib.Configs
{
	public class DroneConfigData
	{
		[JsonProperty("build-dir")]
		public string BuildDir { get; set; }

		[JsonProperty("source-files")]
		public IList<string> SourceFiles { get; set; }

		[JsonProperty("reference-files")]
		public IList<string> ReferenceFiles { get; set; }

		public DroneConfigData()
		{
			this.SourceFiles = new List<string>();
			this.ReferenceFiles = new List<string>();
		}
	}
}