using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib
{
	/// <summary>
	/// Provides access to the config and flags used
	/// when running the drone app
	/// </summary>
	public class DroneEnvironment
	{
		/// <summary>
		/// Gets or sets the configuration loaded by the drone app.
		/// </summary>
		/// <value>
		/// The configuration object.
		/// </value>
		public static DroneConfig Config { get; internal set; }

		/// <summary>
		/// Gets or sets the flags use to execute the drone app.
		/// </summary>
		/// <value>
		/// The flags object.
		/// </value>
		public static DroneFlags Flags { get; internal set; }

		/// <summary>
		/// Pushes a 'scope' of global vars that will 
		/// be unset when the object is disposed
		/// </summary>
		/// <param name="flags">The flags.</param>
		/// <param name="config">The configuration.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">
		/// flags
		/// or
		/// config
		/// </exception>
		internal static IDisposable PushScope(DroneConfig config, DroneFlags flags)
		{
			if (config == null)
				throw new ArgumentNullException("config");

			if(flags == null)
				throw new ArgumentNullException("flags");

			Flags = flags;
			Config = config;
			return new DroneEnvironmentScope();
		}
	}
}