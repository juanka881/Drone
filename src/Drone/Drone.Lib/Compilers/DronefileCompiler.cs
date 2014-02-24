using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Drone.Lib.Repo;

namespace Drone.Lib.Compilers
{
	public class DronefileCompiler
	{
		public DronefileCompilerResult Compile(Dronefile dronefile)
		{
			var filename = Path.GetFileNameWithoutExtension(dronefile.FileName);
			var outputFilename = string.Format("{0}.dll", filename);
			var outputFilepath = Path.Combine(dronefile.BuildDir, outputFilename);
			var workDir = dronefile.FileDir;

			if (!Directory.Exists(dronefile.BuildDir))
				Directory.CreateDirectory(dronefile.BuildDir);

			var args = new CSharpCompilerArgs(workDir, 
				outputFilepath, 
				dronefile.SourceFiles, 
				dronefile.ReferenceFiles);

			var csc = new CSharpCompiler();
			var cscResult = csc.Compile(args);

			var result = new DronefileCompilerResult(cscResult.OutputAssemblyFilepath);
			return result;
		}
	}
}
