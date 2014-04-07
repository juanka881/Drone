using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Drone.Lib.Configs
{
	public class DroneReferenceFileTypeConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DroneReferenceFileType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var str = (string) reader.Value;
			var val = default(DroneReferenceFileType);

			if (str == "gac")
			{
				val = DroneReferenceFileType.GlobalAssemblyCache;
			}
			else if(str == "file")
			{
				val = DroneReferenceFileType.File;
			}
			else
			{
				throw new InvalidOperationException("unable to parse drone reference file type");
			}

			return val;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var val = (DroneReferenceFileType) value;

			switch(val)
			{
				case DroneReferenceFileType.File:
					writer.WriteValue("file");
					break;

				case DroneReferenceFileType.GlobalAssemblyCache:
					writer.WriteValue("gac");
					break;
			}
		}
	}
}