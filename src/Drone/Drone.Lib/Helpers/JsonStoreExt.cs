using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public static class JsonStoreExt
	{
		public static T Load<T>(this JsonStore store, string filename)
		{
			if(store == null)
				throw new ArgumentNullException("store");

			using(var fs = File.OpenRead(filename))
			{
				return store.Load<T>(fs);
			}
		}

		public static void Save<T>(this JsonStore store, string filename, T obj)
		{
			if(store == null)
				throw new ArgumentNullException("store");

			using(var fs = File.Open(filename, FileMode.Create))
			{
				store.Save(fs, obj);
			}
		}
	}
}