using Drone.Lib.FileSystem;
using Drone.Lib.Helpers;
using Drone.Tests.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Drone.Tests.FileSystemFixtures
{
	[TestFixture]
	public class FileInfoCacheFixture
	{
		[Test]
		public void can_create()
		{
			Assert.DoesNotThrow(() => new FileMetadataCache());
		}

		[Test]
		public void can_save_and_load()
		{
			using(var fs = new FileSystemTester())
			{
				var set = new FileMetadataCache();
				var store = new JsonStore();
				var filename = "file-change-set.json";

				fs.Write(filename, s => store.Save(s, set));
				fs.Read(filename, store.Load<FileMetadataCache>);
			}
		}

		[Test]
		public void can_add_item()
		{
			var set = new FileMetadataCache();
			set.Add("file.txt", DateTime.UtcNow, 1000);
		}

		[Test]
		public void can_add_and_save_and_load_again()
		{
			using (var fs = new FileSystemTester())
			{
				var set = new FileMetadataCache();
				var store = new JsonStore();
				var filename = "file-change-set.json";

				var file = fs.Touch("file.txt");
				set.Add(file);

				fs.Write(filename, s => store.Save(s, set));

				var loadedSet = fs.Read(filename, store.Load<FileMetadataCache>);

				Assert.IsNotNull(loadedSet);
				Assert.AreEqual(1, loadedSet.Files.Count);

				var fileinfo = loadedSet.Files.ElementAt(0);

				Assert.AreEqual(file.Name, Path.GetFileName(fileinfo.FilePath));
				Assert.AreEqual(file.Length, fileinfo.Length);
				Assert.AreEqual(file.LastWriteTimeUtc, fileinfo.LastWriteTimeUtc);
			}
		}

		[Test]
		public void modifiying_file_in_set_causes_has_changes_to_return_true()
		{
			using (var fs = new FileSystemTester())
			{
				var set = new FileMetadataCache();
				var store = new JsonStore();
				var filename = "file-change-set.json";

				var file = fs.Touch("file.txt");
				set.Add(file);

				fs.Write(filename, s => store.Save(s, set));

				var loadedSet = fs.Read(filename, store.Load<FileMetadataCache>);

				Assert.IsFalse(loadedSet.HasChanges());

				fs.Touch(file.FullName);

				Assert.IsTrue(loadedSet.HasChanges());
			}
		}
	}
}
