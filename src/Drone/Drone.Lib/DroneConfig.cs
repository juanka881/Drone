using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Drone.Lib.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Drone.Lib
{
	public class DroneConfig
	{
		public static readonly string DefaultFilename = "drone.config";

		public static readonly string BinDirname = "drone.bin";

		public static readonly string AssemblyFilename = "drone.user.dll";

		public static DroneConfig Current;

		[JsonIgnore]
		public string HashId { get; private set; }

		[JsonIgnore]
		public string Filepath { get; private set; }

		[JsonIgnore]
		public string Filename { get; private set; }

		[JsonIgnore]
		public string Dirname { get; private set; }

		[JsonIgnore]
		public string BinDirpath { get; private set; }

		[JsonIgnore]
		public string AssemblyFilepath { get; private set; }

		[JsonProperty("source-files")]
		public IList<string> SourceFiles { get; set; }

		[JsonProperty("reference-files")]
		public IList<string> ReferenceFiles { get; set; }

		[JsonProperty("properties")]
		public JObject Properties { get; set; }

		public DroneConfig()
		{
			this.SourceFiles = new List<string>();
			this.ReferenceFiles = new List<string>();
			this.Properties = new JObject();
		}

		public void SetConfigFilename(string filename)
		{
			if(string.IsNullOrWhiteSpace(filename))
				throw new ArgumentException("filename is empty or null", "filename");

			if(Path.IsPathRooted(filename))
			{
				this.Filename = Path.GetFileName(filename);
				this.Filepath = filename;
			}
			else
			{
				this.Filename = filename;
				this.Filepath = Path.GetFullPath(this.Filename);
			}

			this.HashId = HashHelper.GetHash(Path.GetFullPath(this.Filepath));
			this.Dirname = Path.GetDirectoryName(this.Filepath);
			this.BinDirpath = Path.Combine(Path.GetTempPath(), BinDirname, this.HashId);
			this.AssemblyFilepath = Path.Combine(this.BinDirpath, AssemblyFilename);
		}
	}
}