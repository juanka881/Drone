using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Drone.Lib.Compilers
{
	public class DotNetFramework
	{
		private static readonly string RootDir = @"%WINDIR%\Microsoft.NET\Framework";
		private static readonly string CSharpCompilerBinFilename = "csc.exe";
		private static readonly string MSBuildBinFilename = "msbuild.exe";

		public static readonly DotNetFramework Version40 = new DotNetFramework("v4.0.30319");

		private DotNetFramework(string ver)
		{
			if (string.IsNullOrWhiteSpace(ver))
				throw new ArgumentException("ver is empty or null", "ver");

			this.Version = ver;
			this.FrameworkDir = this.GetFrameworkDir(this.Version);
			this.CSharpCompilerBinFilepath = this.GetCSharpCompilerBinFilepath(this.FrameworkDir);
			this.MSBuildBinFilepath = this.GetMSBuildBinFilepath(this.FrameworkDir);
		}

		public string Version { get; private set; }
		public string FrameworkDir { get; private set; }
		public string CSharpCompilerBinFilepath { get; private set; }
		public string MSBuildBinFilepath { get; private set; }
		
		private string GetFrameworkDir(string ver)
		{
			if (string.IsNullOrWhiteSpace(ver))
				throw new ArgumentException("ver is empty or null", "ver");

			var expandedRootDir = Environment.ExpandEnvironmentVariables(RootDir);
			var frameworkDir = Path.Combine(expandedRootDir, ver);
			return frameworkDir;
		}

		private string GetCSharpCompilerBinFilepath(string frameworkDir)
		{
			var path = Path.Combine(frameworkDir, CSharpCompilerBinFilename);
			return path;
		}

		private string GetMSBuildBinFilepath(string frameworkDir)
		{
			var path = Path.Combine(frameworkDir, MSBuildBinFilename);
			return path;
		}
	}
}