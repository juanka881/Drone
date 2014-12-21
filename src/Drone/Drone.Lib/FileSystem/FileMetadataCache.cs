using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Drone.Lib.FileSystem
{
	public class FileMetadataCache
	{
		public FileMetadataCache()
		{
			this.Files = new HashSet<FileMetadataCacheItem>();
		}

		[JsonProperty("files")]
		public HashSet<FileMetadataCacheItem> Files { get; private set; }

		public void Add(string filePath, DateTime lastWriteTimeUtc, long fileSize)
		{
			if(string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("filePath is empty or null", "filePath");

			this.Files.Add(new FileMetadataCacheItem(filePath, lastWriteTimeUtc, fileSize));
		}

		public void Add(FileInfo fileInfo)
		{
			if(fileInfo == null)
				throw new ArgumentNullException("fileInfo");

			this.Files.Add(FileMetadataCacheItem.New(fileInfo));
		}

		public bool HasChanges()
		{
			foreach(var file in this.Files)
			{
				if(!File.Exists(file.FilePath))
					return true;

				var fileInfo = new FileInfo(file.FilePath);

				if(fileInfo.Length != file.Length) 
					return true;

				if(fileInfo.LastWriteTimeUtc > file.LastWriteTimeUtc)
					return true;
			}

			return false;
		}
	}
}