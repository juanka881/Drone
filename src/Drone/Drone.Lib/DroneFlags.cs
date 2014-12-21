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
		public string ConfigFileName { get; private set; }

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

		/// <summary>
		/// Initializes a new instance of the <see cref="DroneFlags"/> class.
		/// </summary>
		/// <param name="configFileName">filename of the configuration file.</param>
		/// <param name="isDebugEnabled">if set to <c>true</c> [is debug enabled].</param>
		/// <param name="isConsoleLogColorsEnabled">if set to <c>true</c> [is console log colors enabled].</param>
		/// <param name="logLevel">The log level.</param>
		/// <exception cref="System.ArgumentException">configFilename is empty or null;configFileName</exception>
		/// <exception cref="System.ArgumentNullException">logLevel</exception>
		public DroneFlags(string configFileName, bool isDebugEnabled, bool isConsoleLogColorsEnabled, LogLevel logLevel)
		{
			if(string.IsNullOrWhiteSpace(configFileName))
				throw new ArgumentException("configFilename is empty or null", "configFileName");

			if(logLevel == null)
				throw new ArgumentNullException("logLevel");

			this.ConfigFileName = configFileName;
			this.IsDebugEnabled = isDebugEnabled;
			this.IsConsoleLogColorsEnabled = isConsoleLogColorsEnabled;
			this.LogLevel = logLevel;
		}
	}
}