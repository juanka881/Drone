using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib
{
	/// <summary>
	/// Provides access to the config and flags used
	/// when running the drone app
	/// </summary>
	public class DroneEnv
	{
		private static DroneEnv current;

		/// <summary>
		/// Gets the configuration loaded by the drone app.
		/// </summary>
		/// <value>
		/// The configuration object.
		/// </value>
		public DroneConfig Config { get; private set; }
		
		/// <summary>
		/// Gets the flags use to execute the drone app.
		/// </summary>
		/// <value>
		/// The flags object.
		/// </value>
		public DroneFlags Flags { get; private set; }

		public DroneEnv(DroneConfig config, DroneFlags flags)
		{
			if(config == null)
				throw new ArgumentNullException("config");

			if(flags == null)
				throw new ArgumentNullException("flags");

			this.Config = config;
			this.Flags = flags;
		}

		public static DroneEnv Current
		{
			get
			{
				if(current == null)
					throw new NullReferenceException("DroneEnv.Current");

				return current;
			}
		}

		public static void Set(DroneEnv env)
		{
			if(env == null)
				throw new ArgumentNullException("env");

			current = env;
		}
	}
}