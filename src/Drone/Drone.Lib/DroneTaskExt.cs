using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Helpers;

namespace Drone.Lib
{
	/// <summary>
	/// Provides extensions to drone task objects
	/// </summary>
	public static class DroneTaskExt
	{
		/// <summary>
		/// Clones the specified task using a provided 
		/// function to set properties on the new task.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="newName">The new name.</param>
		/// <param name="fn">The function.</param>
		/// <returns></returns>
		public static DroneTask Clone<T>(this T task, string newName, Action<T> fn) where T : DroneTask
		{
			if(task == null)
				throw new ArgumentNullException("task");

			if(fn == null)
				throw new ArgumentNullException("fn");

			if(string.IsNullOrWhiteSpace(newName))
				throw new ArgumentException("newName is empty or null", "newName");

			var taskCopy = Activator.CreateInstance<T>();
			taskCopy.Name = newName;
			taskCopy.Dependencies = task.Dependencies;
			fn(taskCopy);
			return taskCopy;
		}
	}
}