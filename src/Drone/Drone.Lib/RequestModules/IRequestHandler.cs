using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.RequestModules
{
	public interface IRequestHandler
	{
		void Handle(object parameter);
	}
}