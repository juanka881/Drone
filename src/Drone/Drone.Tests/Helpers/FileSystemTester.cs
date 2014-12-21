using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Drone.Tests.Helpers
{
	public class FileSystemTester : IDisposable
	{
		private HashSet<string> files;

		public FileSystemTester()
		{
			this.files = new HashSet<string>();
		}

		public void Write(string filename, Action<Stream> fn)
		{
			using (var stream = File.OpenWrite(filename))
			{
				this.files.Add(filename);
				fn(stream);
			}
		}

		public T Read<T>(string filename, Func<Stream, T> fn)
		{
			using(var stream = File.OpenRead(filename))
			{
				return fn(stream);
			}
		}

		public FileInfo Touch(string filename)
		{
			if(File.Exists(filename))
			{
				File.SetLastWriteTimeUtc(filename, DateTime.UtcNow);
			}
			else
			{
				this.Write(filename, s => { });	
			}
			
			return new FileInfo(filename);
		}

		public void Delete(string filename)
		{
			try
			{
				if (!File.Exists(filename))
					return;

				File.Delete(filename);
				this.files.Remove(filename);
			}
			catch (Exception)
			{

			}
		}

		public void Dispose()
		{
			var filesCopy = this.files.ToList();

			foreach (var file in filesCopy)
				this.Delete(file);
		}
	}
}