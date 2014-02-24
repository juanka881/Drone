using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Drone.Lib.Repo
{
	public class DronefileRepo
	{
		public Dronefile Load(Stream stream, string filepath)
		{
			using (var reader = new StreamReader(stream))
			{
				using (var jsonReader = new JsonTextReader(reader))
				{
					var serializer = new JsonSerializer();
					var data = serializer.Deserialize<DronefileData>(jsonReader);
					var fullpath = Path.GetFullPath(filepath);
					var dronefile = data.ToDronefile(fullpath);
					return dronefile;	
				}
			}
		}

		public void Save(Dronefile dronefile, Stream stream)
		{
			using (var writer = new StreamWriter(stream))
			{
				using (var jsonWriter = new JsonTextWriter(writer))
				{
					var serializer = new JsonSerializer();
					serializer.Formatting = Formatting.Indented;
					var data = dronefile.ToDronefileData();
					serializer.Serialize(jsonWriter, data);		
				}
			}
		}
	}
}