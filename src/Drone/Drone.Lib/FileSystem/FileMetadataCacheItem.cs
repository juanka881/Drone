using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Drone.Lib.FileSystem
{
	public class FileMetadataCacheItem
	{
		[JsonProperty("file-path")]
		public string FilePath { get; private set; }

		[JsonProperty("last-write-time-utc")]
		public DateTime LastWriteTimeUtc { get; private set; }

		[JsonProperty("length")]
		public long Length { get; private set; }

		public FileMetadataCacheItem(string filePath, DateTime lastWriteTimeUtc, long length)
		{
			if(string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("filePath is empty or null", "filePath");

			this.FilePath = filePath;
			this.LastWriteTimeUtc = lastWriteTimeUtc;
			this.Length = length;
		}

		public static FileMetadataCacheItem New(FileInfo fileInfo)
		{
			if(fileInfo == null)
				throw new ArgumentNullException("fileInfo");

			return new FileMetadataCacheItem(fileInfo.FullName, fileInfo.LastWriteTimeUtc, fileInfo.Length);
		}
	}
}