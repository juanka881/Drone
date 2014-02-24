using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Exceptions;

namespace Drone.Lib.Engine
{
	public class DroneMainModuleFinder
	{
		public DroneModule Find()
		{
			var moduleTypes = (from asm in AppDomain.CurrentDomain.GetAssemblies()
							   from type in asm.GetTypes()
							   where typeof (DroneModule).IsAssignableFrom(type) &&
									 type.Name.ToLower() == DroneModule.MainModuleName
							   select type).ToList();

			if (moduleTypes.Count > 0)
			{
				if (moduleTypes.Count > 1)
					throw TooManyMainModulesFoundException.Get(moduleTypes);

				var moduleType = moduleTypes[0];
				var module = Activator.CreateInstance(moduleType) as DroneModule;
				return module;
			}
			else
			{
				throw MainModuleNotFoundException.Get();
			}
		}
	}
}