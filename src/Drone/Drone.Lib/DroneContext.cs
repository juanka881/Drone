using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib
{
	/// <summary>
	/// Provides access to the config and flags used
	/// when running the drone app
	/// </summary>
	public class DroneContext
	{
		private static DroneConfig configInstance;
		private static DroneFlags flagsInstance;

		/// <summary>
		/// Gets the configuration loaded by the drone app.
		/// </summary>
		/// <value>
		/// The configuration object.
		/// </value>
		public static DroneConfig Config
		{
			get
			{
				if(configInstance == null)
					throw new NullReferenceException("configInstance");

				return configInstance;
			}
		}

		/// <summary>
		/// Gets the flags use to execute the drone app.
		/// </summary>
		/// <value>
		/// The flags object.
		/// </value>
		public static DroneFlags Flags
		{
			get
			{
				if(flagsInstance == null)
					throw new NullReferenceException("flagsInstance");

				return flagsInstance;
			}
		}

		internal static IDisposable Set(DroneConfig config, DroneFlags flags)
		{
			if (config == null)
				throw new ArgumentNullException("config");

			if(flags == null)
				throw new ArgumentNullException("flags");

			flagsInstance = flags;
			configInstance = config;
			return new DroneContextScope();
		}

		internal static void Unset()
		{
			flagsInstance = null;
			configInstance = null;
		}
	}
}