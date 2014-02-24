using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.RequestModules.FileModule
{
	public class AddParameter : RequestParameter
	{
		public IList<string> Files { get; private set; }
		
		public AddParameter(IList<string> files)
		{
			if (files == null)
				throw new ArgumentNullException("files");

			this.Files = new List<string>(files);
		}
	}
}