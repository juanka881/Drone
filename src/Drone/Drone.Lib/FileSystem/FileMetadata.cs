using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Drone.Lib.FileSystem
{
	public class FileMetadata
	{
		public string FullPath { get; private set; }

		public string FilenameWithExt { get; private set; }

		public string FilenameOnly { get; private set; }

		public string FileExt { get; private set; }

		public string FileDir { get; private set; }

		public string OriginalFilename { get; private set; }

		public bool Exists
		{
			get
			{
				return File.Exists(this.FullPath);
			}
		}

		public FileMetadata(string filename, string rootDir = null)
		{
			this.Set(filename, rootDir);
		}

		public void Set(string filename, string rootDir = null)
		{
			if (string.IsNullOrWhiteSpace(filename))
				throw new ArgumentException("filename is empty or null", "filename");

			this.OriginalFilename = filename;

			if(!string.IsNullOrWhiteSpace(rootDir) && !Path.IsPathRooted(filename))
				this.FullPath = Path.GetFullPath(Path.Combine(rootDir, filename));
			else
				this.FullPath = Path.GetFullPath(this.OriginalFilename);

			this.FilenameWithExt = Path.GetFileName(this.FullPath);
			this.FilenameOnly = Path.GetFileNameWithoutExtension(this.FilenameWithExt);
			this.FileExt = Path.GetExtension(this.FilenameWithExt);
			this.FileDir = Path.GetDirectoryName(this.FullPath);
			this.OnSet();
		}

		protected virtual void OnSet()
		{
			
		}
	}
}