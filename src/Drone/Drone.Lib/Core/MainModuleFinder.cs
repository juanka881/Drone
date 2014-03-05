using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Drone.Lib.Exceptions;

namespace Drone.Lib.Core
{
	public class MainModuleFinder
	{
		public DroneModule Find(Assembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");

			var moduleTypes = (from type in assembly.GetTypes()
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