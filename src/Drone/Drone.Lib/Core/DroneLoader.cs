using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Drone.Lib.Configs;
using NLog;

namespace Drone.Lib.Core
{
	public class DroneLoader
	{
		public Logger Log { get; set; }

		public DroneModule Load(DroneConfig config)
		{
			if (!File.Exists(config.AssemblyFilepath))
				throw DroneAssemblyNotFoundException.Get(config.AssemblyFilepath);

			var assembly = null as Assembly;

			this.Log.Debug("attempting to load drone module assembly: '{0}'", Path.GetFileName(config.AssemblyFilepath));

			try
			{
				assembly = Assembly.LoadFrom(config.AssemblyFilepath);
				this.Log.Debug("assembly loaded");
			}
			catch (Exception ex)
			{
				throw DroneAssemblyLoadException.Get(config.AssemblyFilepath, ex);
			}

			this.Log.Debug("searching for main drone module...");

			var moduleTypes = (from type in assembly.GetTypes()
							   where typeof(DroneModule).IsAssignableFrom(type) &&
									 type.Name.ToLower() == DroneModule.MainModuleName
							   select type).ToList();

			if (moduleTypes.Count > 0)
			{
				this.Log.Debug("found {0} drone module{1}", moduleTypes.Count, moduleTypes.Count > 1 ? "s" : string.Empty);

				if(moduleTypes.Count > 1)
					throw DroneTooManyMainModulesFoundException.Get(moduleTypes);

				var moduleType = moduleTypes[0];

				this.Log.Debug("creating drone module instance from type: '{0}'", moduleType.FullName);

				var module = Activator.CreateInstance(moduleType) as DroneModule;

				this.Log.Debug("drone module created");

				return module;
			}
			else
			{
				this.Log.Debug("no drone modules found");

				throw DroneMainModuleNotFoundException.Get();
			}
		}
	}
}