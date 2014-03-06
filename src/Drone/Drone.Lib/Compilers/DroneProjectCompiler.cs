using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Drone.Lib.Configs;

namespace Drone.Lib.Compilers
{
	public class DroneProjectCompiler
	{
		public DroneProjectCompilerResult Compile(DroneProject project)
		{
			//var filename = Path.GetFileNameWithoutExtension(project.FileName);
			//var outputFilename = string.Format("{0}.dll", filename);
			//var outputFilepath = Path.Combine(project.BuildDir, outputFilename);
			//var workDir = project.FileDir;

			//if (!Directory.Exists(project.BuildDir))
			//	Directory.CreateDirectory(project.BuildDir);

			//var args = new CSharpCompilerArgs(workDir, 
			//	outputFilepath, 
			//	project.SourceFiles, 
			//	project.ReferenceFiles);

			//var csc = new CSharpCompiler();
			//var cscResult = csc.Compile(args);

			//var result = DroneProjectCompilerResult.GetSuccess(cscResult.OutputAssemblyFilepath);
			//return result;
			throw new NotImplementedException();
		}
	}
}
