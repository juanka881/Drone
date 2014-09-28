using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.FileSystem
{
	public class FileChangeSet
	{
		public FileChangeSet(string baseDir)
		{
			
		}

		public string BaseDir { get; set; }

		public HashSet<string> Files { get; private set; }

		public void Add(string file)
		{
			this.Files.Add(file);
		}
	}

	public class FileChangeSetItem
	{
		public string Filename { get; private set; }

		public DateTime Timestamp { get; set; }

		public FileChangeSetItem(string filename, DateTime timestamp)
		{
			if(string.IsNullOrWhiteSpace(filename))
				throw new ArgumentException("filename is empty or null", "filename");

			this.Filename = filename;

			this.Timestamp = timestamp;
		}
	}
}