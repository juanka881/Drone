using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Drone.Lib.Helpers
{
	public class JsonStore
	{
		private readonly JsonSerializer serializer;

		public JsonStore()
		{
			this.serializer = new JsonSerializer();
			this.serializer.Formatting = Formatting.Indented;
		}

		public T Load<T>(Stream stream)
		{
			using (var reader = new StreamReader(stream))
			{
				using (var jsonReader = new JsonTextReader(reader))
				{
					var item = this.serializer.Deserialize<T>(jsonReader);
					return item;
				}
			}
		}

		public void Save<T>(Stream stream, T obj)
		{
			using (var writer = new StreamWriter(stream))
			{
				using (var jsonWriter = new JsonTextWriter(writer))
				{
					this.serializer.Serialize(jsonWriter, obj);
				}
			}
		}
	}
}