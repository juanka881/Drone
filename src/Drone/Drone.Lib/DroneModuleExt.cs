using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Tasks;

namespace Drone.Lib
{
	/// <summary>
	/// Provides extension methods to drone module objects
	/// </summary>
	public static class DroneModuleExt
	{
		/// <summary>
		/// Includes the module into this module without any namespace prefix.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <param name="otherModule">The other module.</param>
		/// <exception cref="System.ArgumentNullException">
		/// module
		/// or
		/// otherModule
		/// </exception>
		public static void Include(this DroneModule module, DroneModule otherModule)
		{
			if(module == null)
				throw new ArgumentNullException("module");

			if(otherModule == null)
				throw new ArgumentNullException("otherModule");

			module.Include(string.Empty, otherModule);
		}

		/// <summary>
		/// Registers the given action as a task to execute with dependencies.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <param name="name">The name of the task.</param>
		/// <param name="deps">The dependencies of the task.</param>
		/// <param name="fn">The function to invoke when the task is executed.</param>
		/// <exception cref="System.ArgumentNullException">module</exception>
		public static void Task(this DroneModule module, string name, IEnumerable<string> deps, Action<DroneTaskContext> fn)
		{
			if(module == null)
				throw new ArgumentNullException("module");

			module.Add(new ActionTask(name, deps, fn));
		}

		/// <summary>
		/// Registers the given action as a task to execute.
		/// </summary>
		/// <param name="module">The module.</param>
		/// <param name="name">The name of the task.</param>
		/// <param name="fn">The function to invoke when the task is executed.</param>
		public static void Task(this DroneModule module, string name, Action<DroneTaskContext> fn)
		{
			Task(module, name, DroneTask.NoDeps, fn);
		}

		/// <summary>
		/// Registers a task name alias that will execute all the given
		/// dependencies when this task is executed
		/// </summary>
		/// <param name="module">The module.</param>
		/// <param name="name">The name of the task alias.</param>
		/// <param name="deps">The list of task names to execute.</param>
		/// <exception cref="System.ArgumentNullException">
		/// module
		/// or
		/// deps
		/// </exception>
		public static void Alias(this DroneModule module, string name, IEnumerable<string> deps)
		{
			if(module == null)
				throw new ArgumentNullException("module");

			if(deps == null)
				throw new ArgumentNullException("deps");

			module.Add(new AliasTask(name, deps));
		}
	}
}