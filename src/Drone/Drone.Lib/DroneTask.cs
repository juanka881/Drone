using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib
{
	/// <summary>
	/// Represents a drone task. a task is the basis for all operations in a drone app.
	/// tasks are organize by modules, drone uses the main module as the entry point 
	/// to look for tasks to execute.
	/// </summary>
	public class DroneTask
	{
		/// <summary>
		/// Provides an empty list of dependencies
		/// </summary>
		public static readonly IEnumerable<string> NoDeps = Enumerable.Empty<string>();

		/// <summary>
		/// Gets the name of the task
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		public string Name { get; internal set; }

		/// <summary>
		/// Gets the dependencies of the task. dependencies are tasks
		/// that will be executed prior to executing this task
		/// </summary>
		/// <value>
		/// The dependencies.
		/// </value>
		public IList<string> Dependencies { get; internal set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DroneTask"/> class.
		/// </summary>
		public DroneTask()
		{
			this.Name = this.GetType().Name;
			this.Dependencies = new List<string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DroneTask"/> class.
		/// </summary>
		/// <param name="name">The name of the task.</param>
		/// <param name="deps">The dependencies of the task, if any.</param>
		/// <exception cref="System.ArgumentException">name is empty or null;name</exception>
		/// <exception cref="System.ArgumentNullException">deps</exception>
		public DroneTask(string name, IEnumerable<string> deps)
		{
			if(string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("name is empty or null", "name");

			if(deps == null)
				throw new ArgumentNullException("deps");

			this.Name = name;
			this.Dependencies = new List<string>(deps);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DroneTask"/> class.
		/// </summary>
		/// <param name="name">The name of the task.</param>
		public DroneTask(string name) : this(name, Enumerable.Empty<string>())
		{
			
		}

		/// <summary>
		/// Creates a copy of the task, derived task should 
		/// override this method to ensure all property values are 
		/// correctly clone.
		/// </summary>
		/// <param name="newName">The new name of the task.</param>
		/// <returns>an copy of this task object</returns>
		public virtual DroneTask Clone(string newName)
		{
			return this.Clone(newName, x => { });
		}
	}
}
