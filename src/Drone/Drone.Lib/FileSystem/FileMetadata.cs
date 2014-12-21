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

		public string FileNameWithExt { get; private set; }

		public string FileName { get; private set; }

		public string FileExt { get; private set; }

		public string FileDir { get; private set; }

		public bool IsRelative { get; private set; }

		public string OriginalValue { get; private set; }

		public bool Exists
		{
			get
			{
				return File.Exists(this.FullPath);
			}
		}

		public FileMetadata(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");

			this.OriginalValue = filePath;
			this.IsRelative = !Path.IsPathRooted(filePath);
			this.FullPath = Path.GetFullPath(filePath);
			this.FileNameWithExt = Path.GetFileName(filePath);
			this.FileName = Path.GetFileNameWithoutExtension(filePath);
			this.FileExt = Path.GetExtension(filePath);
			this.FileDir = Path.GetDirectoryName(this.FullPath);
		}
	}
}