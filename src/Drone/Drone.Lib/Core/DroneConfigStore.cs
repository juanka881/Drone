using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Drone.Lib.Core
{
	public class DroneConfigStore
	{
		public DroneConfig Load(Stream stream, string filepath)
		{
			using (var reader = new StreamReader(stream))
			{
				using (var jsonReader = new JsonTextReader(reader))
				{
					var serializer = new JsonSerializer();
					var config = serializer.Deserialize<DroneConfig>(jsonReader);
					var fullpath = Path.GetFullPath(filepath);
					config.SetConfigFilename(fullpath);
					return config;	
				}
			}
		}

		public void Save(DroneConfig config, Stream stream)
		{
			using (var writer = new StreamWriter(stream))
			{
				using (var jsonWriter = new JsonTextWriter(writer))
				{
					var serializer = new JsonSerializer();
					serializer.Formatting = Formatting.Indented;
					serializer.Serialize(jsonWriter, config);
				}
			}
		}
	}
}