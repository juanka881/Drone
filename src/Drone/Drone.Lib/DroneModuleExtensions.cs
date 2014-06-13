using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Core;

namespace Drone.Lib
{
	public static class DroneModuleExtensions
	{
		public static void Add(this DroneModule module, DroneModule otherModule)
		{
			if(module == null)
				throw new ArgumentNullException("module");

			if(otherModule == null)
				throw new ArgumentNullException("otherModule");

			module.Add(string.Empty, otherModule);
		}

		public static void Add(this DroneModule module, 
			string name, 
			Action<DroneTaskContext> action, 
			params string[] deps)
		{
			if(module == null)
				throw new ArgumentNullException("module");

			module.Add(new DroneActionTask(name, deps, action));
		}

		public static void Add(this DroneModule module, 
			string name, 
			params string[] deps)
		{
			if(module == null)
				throw new ArgumentNullException("module");

			if(deps == null)
				throw new ArgumentNullException("deps");

			module.Add(new DroneDependencyOnlyTask(name, deps));
		}

		public static void AddFn(this DroneModule module, 
			string name, 
			Func<DroneTaskContext, DroneTask> factory, 
			params string[] deps)
		{
			if(module == null)
				throw new ArgumentNullException("module");

			module.Add(new DroneFactoryTask(name, deps, factory));
		}

		public static void AddSet(this DroneModule module, 
			string name, 
			Func<DroneTaskContext, IEnumerable<DroneTask>> factory,
			params string[] deps)
		{
			if(module == null)
				throw new ArgumentNullException("module");

			module.Add(new DroneSetFactoryTask(name, deps, factory));
		}
	}
}