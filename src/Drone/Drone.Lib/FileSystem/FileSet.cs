using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.FileSystem
{
	public class FileSet
	{
		public FileSet()
		{
			this.BaseDir = string.Empty;
			this.IncludePatterns = new HashSet<string>();
			this.ExcludePatterns = new HashSet<string>();
		}

		public HashSet<string> IncludePatterns { get; set; }

		public HashSet<string> ExcludePatterns { get; set; }

		public string BaseDir { get; set; }

		public FileSet Include(string pattern)
		{
			this.IncludePatterns.Add(pattern);
			return this;
		}

		public FileSet Exclude(string pattern)
		{
			this.ExcludePatterns.Add(pattern);
			return this;
		}

		public FileSet Base(string baseDir)
		{
			if(string.IsNullOrWhiteSpace(baseDir))
				throw new ArgumentException("baseDir is empty or null", "baseDir");

			this.BaseDir = baseDir;
			return this;
		}

		public FileSet Clone()
		{
			var copy = new FileSet();

			copy.BaseDir = this.BaseDir;
			copy.IncludePatterns = new HashSet<string>(this.IncludePatterns);
			copy.ExcludePatterns = new HashSet<string>(this.ExcludePatterns);
			 
			return copy;
		}
	}
}