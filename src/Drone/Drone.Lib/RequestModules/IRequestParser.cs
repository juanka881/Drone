using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.RequestModules
{
	public interface IRequestParser
	{
		object GetParameter(IList<string> tokens);
	}
}