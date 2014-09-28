using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace Drone.Lib
{
	/// <summary>
	/// Represents the flags used to execute the drone app
	/// </summary>
	public class DroneFlags
	{
		/// <summary>
		/// Gets or sets the configuration filename.
		/// </summary>
		/// <value>
		/// The configuration filename.
		/// </value>
		public string ConfigFilename { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether debug is enabled.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsDebugEnabled { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether console log colors enabled.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is console log colors disabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsConsoleLogColorsEnabled { get; private set; }

		/// <summary>
		/// Gets or sets the log level.
		/// </summary>
		/// <value>
		/// The log level.
		/// </value>
		public LogLevel LogLevel { get; private set; }

		public DroneFlags(string configFilename, bool isDebugEnabled, bool isConsoleLogColorsEnabled, LogLevel logLevel)
		{
			if(string.IsNullOrWhiteSpace(configFilename))
				throw new ArgumentException("configFilename is empty or null", "configFilename");

			if(logLevel == null)
				throw new ArgumentNullException("logLevel");

			this.ConfigFilename = configFilename;
			this.IsDebugEnabled = isDebugEnabled;
			this.IsConsoleLogColorsEnabled = isConsoleLogColorsEnabled;
			this.LogLevel = logLevel;
		}
	}
}