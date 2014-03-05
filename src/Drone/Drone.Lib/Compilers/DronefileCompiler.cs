using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Drone.Lib.Configs;

namespace Drone.Lib.Compilers
{
	public class DronefileCompiler
	{
		public DronefileCompilerResult Compile(DroneConfig droneConfig)
		{
			var filename = Path.GetFileNameWithoutExtension(droneConfig.FileName);
			var outputFilename = string.Format("{0}.dll", filename);
			var outputFilepath = Path.Combine(droneConfig.BuildDir, outputFilename);
			var workDir = droneConfig.FileDir;

			if (!Directory.Exists(droneConfig.BuildDir))
				Directory.CreateDirectory(droneConfig.BuildDir);

			var args = new CSharpCompilerArgs(workDir, 
				outputFilepath, 
				droneConfig.SourceFiles, 
				droneConfig.ReferenceFiles);

			var csc = new CSharpCompiler();
			var cscResult = csc.Compile(args);

			var result = new DronefileCompilerResult(cscResult.OutputAssemblyFilepath);
			return result;
		}
	}
}
