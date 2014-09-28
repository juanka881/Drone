using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib
{
	/// <summary>
	/// Represents the state of execution a drone task
	/// </summary>
	public enum DroneTaskState
	{
		NotRan,
		Faulted,
		Completed
	}
}