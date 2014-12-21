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
		public static readonly string DefaultFileName = "drone.json";

		public static readonly string BuildDirName = "drone-builds";

		public static readonly string AssemblyFileName = "DroneUserTasks.dll";

		[JsonIgnore]
		public string HashId { get; private set; }

		[JsonIgnore]
		public string FilePath { get; private set; }

		[JsonIgnore]
		public string FileName { get; private set; }

		[JsonIgnore]
		public string DirName { get; private set; }

		[JsonIgnore]
		public string BuildDirPath { get; private set; }

		[JsonIgnore]
		public string AssemblyFilePath { get; private set; }

		[JsonProperty("source-files")]
		public IList<string> SourceFiles { get; private set; }

		[JsonProperty("reference-files")]
		public IList<string> ReferenceFiles { get; private set; }

		[JsonProperty("props")]
		public JObject Props { get; private set; }

		public DroneConfig()
		{
			this.SourceFiles = new List<string>();
			this.ReferenceFiles = new List<string>();
			this.Props = new JObject();
		}

		public void SetConfigFilename(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");

			this.FilePath = Path.GetFullPath(filePath);
			this.FileName = Path.GetFileName(filePath);
			this.HashId = HashHelper.GetHash(this.FilePath);
			this.DirName = Path.GetDirectoryName(this.FilePath);
			this.BuildDirPath = Path.Combine(Path.GetTempPath(), BuildDirName, this.HashId);
			this.AssemblyFilePath = Path.Combine(this.BuildDirPath, AssemblyFileName);
		}
	}
}