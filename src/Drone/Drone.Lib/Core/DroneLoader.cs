using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;

namespace Drone.Lib.Core
{
	public class DroneLoader
	{
		private readonly Logger log;

		public DroneLoader()
		{
			this.log = DroneLogManager.GetLog();
		}

		public DroneModule Load(DroneConfig config)
		{
			if (!File.Exists(config.AssemblyFilePath))
				throw DroneAssemblyNotFoundException.Get(config.AssemblyFilePath);

			var assembly = null as Assembly;

			this.log.Debug("attempting to load drone module assembly: '{0}'", Path.GetFileName(config.AssemblyFilePath));

			try
			{
				assembly = Assembly.LoadFrom(config.AssemblyFilePath);
				this.log.Debug("assembly loaded");
			}
			catch (Exception ex)
			{
				throw DroneAssemblyLoadException.Get(config.AssemblyFilePath, ex);
			}

			this.log.Debug("searching for main drone module...");

			var moduleTypes = (from type in assembly.GetTypes()
							   where typeof(DroneModule).IsAssignableFrom(type) &&
									 type.Name.ToLower() == DroneModule.MainModuleName.ToLower()
							   select type).ToList();

			if (moduleTypes.Count > 0)
			{
				this.log.Debug("found {0} drone module{1}", moduleTypes.Count, moduleTypes.Count > 1 ? "s" : string.Empty);

				if(moduleTypes.Count > 1)
					throw DroneTooManyMainModulesFoundException.Get(moduleTypes);

				var moduleType = moduleTypes[0];

				this.log.Debug("creating drone module instance from type: '{0}'", moduleType.FullName);

				try
				{
					var module = Activator.CreateInstance(moduleType) as DroneModule;
					this.log.Debug("drone module created");
					return module;
				}
				catch(Exception ex)
				{
					throw DroneCreateMainModuleFailedException.Get(ex, moduleType);
				}
			}
			else
			{
				this.log.Debug("no drone modules found");

				throw DroneMainModuleNotFoundException.Get();
			}
		}
	}
}