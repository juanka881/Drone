using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Drone.Lib.DotNet
{
	public class DotNetFramework
	{
		private static readonly string RootDirPath = @"%WINDIR%\Microsoft.NET\Framework";
		private static readonly string CSharpCompilerBinFilename = "csc.exe";
		private static readonly string MSBuildBinFilename = "msbuild.exe";

		public static readonly DotNetFramework Version40 = new DotNetFramework("v4.0.30319");

		private DotNetFramework(string ver)
		{
			if (string.IsNullOrWhiteSpace(ver))
				throw new ArgumentException("ver is empty or null", "ver");

			this.Version = ver;
			this.FrameworkDirPath = this.GetFrameworkDir(this.Version);
			this.CSharpCompilerBinFilePath = Path.Combine(this.FrameworkDirPath, CSharpCompilerBinFilename);
			this.MSBuildBinFilePath = Path.Combine(this.FrameworkDirPath, MSBuildBinFilename);
		}

		public string Version { get; private set; }

		public string FrameworkDirPath { get; private set; }

		public string CSharpCompilerBinFilePath { get; private set; }

		public string MSBuildBinFilePath { get; private set; }
		
		private string GetFrameworkDir(string ver)
		{
			if (string.IsNullOrWhiteSpace(ver))
				throw new ArgumentException("ver is empty or null", "ver");

			var expandedRootDir = Environment.ExpandEnvironmentVariables(RootDirPath);
			var frameworkDir = Path.Combine(expandedRootDir, ver);
			return frameworkDir;
		}
	}
}