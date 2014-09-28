using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Drone.Lib.Tasks
{
	public class CopyFileTaskHanlder : DroneTaskHandler<CopyFileTask>
	{
		public override void Handle(CopyFileTask task, DroneTaskContext context)
		{
			//foreach(var src in task.Sources)
			//{
			//	this.CopyCore(src, task.Destination, (s, d) =>
			//	{
			//		context.Log.Info("{0} => {1}", s, d);
			//	});
			//}
		}

		private string GetRelativePath(string fullpath, string rootpath)
		{
			return fullpath.Replace(rootpath, string.Empty).Remove(0, 1);
		}

		private void CopyCore(string sourcePath, string destinationPath, Action<string, string> onCopied)
		{
			if(File.Exists(sourcePath))
			{
				var filename = Path.GetFileName(sourcePath);
				var destinationFilepath = Path.Combine(destinationPath, filename);

				File.Copy(sourcePath, destinationFilepath);

				if(onCopied != null)
					onCopied(sourcePath, destinationFilepath);
			}
			else if(Directory.Exists(sourcePath))
			{
				var sourceDirpath = Path.GetFullPath(sourcePath);
				var destinationDirpath = Path.GetFullPath(destinationPath);
				var files = Directory.EnumerateFiles(sourceDirpath, "*", SearchOption.AllDirectories);
				
				foreach(var sourceFilepath in files)
				{
					var relativeFilepath = this.GetRelativePath(sourceFilepath, sourceDirpath);
					var filePath = Path.Combine(destinationDirpath, relativeFilepath);
					var fileDirpath = Path.GetDirectoryName(filePath);

					if(!Directory.Exists(fileDirpath))
						Directory.CreateDirectory(fileDirpath);

					File.Copy(sourceFilepath, filePath, true);

					if (onCopied != null)
						onCopied(sourceFilepath, filePath);
				}
			}
		}
	}
}