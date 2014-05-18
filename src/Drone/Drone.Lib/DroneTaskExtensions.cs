using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib
{
	public static class DroneTaskExtensions
	{
		public static DroneTask Clone<T>(this T task, string newName, Action<T> fn) where T : DroneTask
		{
			var newTask = task.Clone(newName);
			fn(newTask as T);
			return newTask;
		}
	}
}