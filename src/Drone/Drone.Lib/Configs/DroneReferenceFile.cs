using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Converters;

namespace Drone.Lib.Configs
{
	public class DroneReferenceFile
	{
		[JsonProperty("type")]
		[JsonConverter(typeof(DroneReferenceFileTypeConverter))]
		public DroneReferenceFileType Type { get; private set; }

		[JsonProperty("path")]
		public string Path { get; private set; }

		public DroneReferenceFile(DroneReferenceFileType type, string path)
		{
			if(string.IsNullOrWhiteSpace(path))
				throw new ArgumentException("path is empty or null", "path");

			this.Type = type;
			this.Path = path;
		}

		public override int GetHashCode()
		{
			var path = this.Path ?? string.Empty;

			var hash = 17;
			hash = hash * 23 + path.GetHashCode();
			hash = hash * 23 + this.Type.GetHashCode();

			return hash;
		}

		public override bool Equals(object obj)
		{
			var other = obj as DroneReferenceFile;

			if(other == null)
				return false;

			var thisPath = (this.Path ?? string.Empty).ToLower();
			var otherPath = (other.Path ?? string.Empty).ToLower();

			return thisPath == otherPath && this.Type == other.Type;
		}
	}
}