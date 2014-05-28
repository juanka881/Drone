using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.App.CommandHandlers;
using Drone.Lib;
using Drone.Lib.Configs;
using Drone.Lib.Core;
using Drone.Lib.Helpers;
using NLog;
using Autofac;
using NLog.Config;
using NLog.Targets;
using System.IO;

namespace Drone.App.Core
{
	public class CommandRunner
	{
		private static readonly LogLevel[] logLevels = 
		{
			LogLevel.Fatal,
			LogLevel.Error,
			LogLevel.Warn,
			LogLevel.Info,
			LogLevel.Debug,
			LogLevel.Trace
		};

		private readonly IContainer container;
		private readonly Logger log;
		
		public CommandRunner()
		{
			this.container = this.GetContainer();
			this.log = this.container.Resolve<Logger>();
		}

		private StringTokenSet GetTokens(string commandString)
		{
			var tokenizer = new StringTokenizer();
			var tokens = tokenizer.GetTokens(commandString);
			return new StringTokenSet(tokens);
		}

		private CommandHandler GetHandler(StringTokenSet tokens)
		{
			var requestToken = tokens.Pop();
			var request = "help";

			var commandLineArgs = Environment.GetCommandLineArgs();

			if(requestToken != null && commandLineArgs != null && commandLineArgs.Length >= 1 && requestToken.Value == commandLineArgs[0])
				requestToken = tokens.Pop();

			if (requestToken != null)
				request = requestToken.Value;

			var handler = null as object;

			if (!this.container.TryResolveKeyed(request, typeof(CommandHandler), out handler))
				throw CommandHandlerNotFoundException.Get(request);

			return (CommandHandler)handler;
		}

		private LogLevel GetLogLevelFromString(string str)
		{
			if(string.IsNullOrWhiteSpace(str))
				throw InvalidLogLevelStringException.Get(str);

			var level = null as LogLevel;
			
			switch(str.ToLower())
			{
				case "off":
					level = LogLevel.Off;
					break;

				case "fatal":
					level = LogLevel.Fatal;
					break;

				case "error":
				case "err":
					level = LogLevel.Error;
					break;

				case "warn":
					level = LogLevel.Warn;
					break;
					
				case "info":
					level = LogLevel.Info;
					break;
					
				case "debug":
					level = LogLevel.Debug;
					break;

				case "trace":
					level = LogLevel.Trace;
					break;
			}

			if(level == null)
				throw InvalidLogLevelStringException.Get(str);

			return level;
		}

		private DroneFlags GetFlags(StringTokenSet tokens)
		{
			var flags = new DroneFlags();

			flags.ConfigFilename = tokens.GetFlagValueAndRemove("-f", DroneConfig.DefaultFilename);
			flags.IsDebugEnabled = tokens.GetFlagAndRemove("-d");
			flags.IsConsoleLogColorsDisabled = tokens.GetFlagAndRemove("--no-colors");
			flags.LogLevel = this.GetLogLevelFromString(tokens.GetFlagValueAndRemove("-l", "info"));

			return flags;
		}

		private void ApplyLogConfig(DroneFlags flags)
		{
			if(flags == null)
				throw new ArgumentNullException("flags");

			var rules = (from rule in LogManager.Configuration.LoggingRules
						 where rule.LoggerNamePattern == "drone" || rule.LoggerNamePattern == "drone.task.*"
						 select rule).ToList();

			foreach(var rule in rules)
			{
				var level = flags.LogLevel;

				if(rule.LoggerNamePattern == "drone" && level > LogLevel.Info)
					level = LogLevel.Info;

				var effectiveLevels = logLevels.Where(l => l >= level).ToList();

				this.ApplyLogLevel(rule, effectiveLevels);
			}
			
			if(flags.IsConsoleLogColorsDisabled)
				this.DisableConsoleLogColors(rules);

			var errorFileTarget = (from target in LogManager.Configuration.AllTargets
								   where target.Name == "drone.error.file" && target is FileTarget
								   select target as FileTarget).FirstOrDefault();

			if(errorFileTarget != null)
			{
				var configPath = Path.GetFullPath(flags.ConfigFilename);
				var configDir = Path.GetDirectoryName(configPath);
				errorFileTarget.FileName = Path.Combine(configDir, "drone.errors.txt");
			}

			LogManager.ReconfigExistingLoggers();
		}

		private void ApplyLogLevel(LoggingRule rule, IList<LogLevel> levels)
		{
			foreach(var level in rule.Levels)
				rule.DisableLoggingForLevel(level);

			foreach(var level in levels)
				rule.EnableLoggingForLevel(level);
		}

		private void DisableConsoleLogColors(IList<LoggingRule> rules)
		{
			foreach(var rule in rules)
			{
				foreach(var target in rule.Targets.OfType<ColoredConsoleTarget>())
				{
					target.UseDefaultRowHighlightingRules = false;
					target.RowHighlightingRules.Clear();
					target.WordHighlightingRules.Clear();
				}
			}
		}

		public void Run(string commandString)
		{
			var handler = null as CommandHandler;

			try
			{
				DroneLogHelper.DeleteErrorLogFile();

				var tokens = this.GetTokens(commandString);
				var flags = this.GetFlags(tokens);

				DroneFlags.Current = flags;

				this.ApplyLogConfig(flags);

				handler = this.GetHandler(tokens);
				handler.Flags = flags;

				handler.Handle(tokens);	
			}
			catch (Exception ex)
			{
				this.log.ExceptionAndData(ex);
			}
		}

		private IContainer GetContainer()
		{
			var builder = new ContainerBuilder();

			// core services
			builder.Register(c => DroneLogHelper.GetLog()).AsSelf();
			builder.Register(c => new DroneConfigRepo()).AsSelf();

			builder.Register(c => new DroneCompiler())
				.AsSelf()
				.PropertiesAutowired();

			builder.Register(c => new DroneLoader())
				.AsSelf()
				.PropertiesAutowired();

			builder.Register(c => new DroneTaskRunner(c.Resolve<AutofacDroneTaskHandlerFactory>()))
				.AsSelf()
				.PropertiesAutowired();

			builder.Register(c =>
			{
				var scope = c.Resolve<ILifetimeScope>();
				return new AutofacDroneTaskHandlerFactory(scope);
			}).SingleInstance();

			// handlers
			builder.Register(c => new InitHandler())
				.Keyed<CommandHandler>("init")
				.PropertiesAutowired();

			builder.Register(c => new HelpHandler())
				.Keyed<CommandHandler>("help")
				.PropertiesAutowired();

			builder.Register(c => new AddHandler())
				.Keyed<CommandHandler>("add")
				.PropertiesAutowired();

			builder.Register(c => new RemoveHandler())
				.Keyed<CommandHandler>("rm")
				.PropertiesAutowired();

			builder.Register(c => new RunHandler())
				.Keyed<CommandHandler>("r")
				.PropertiesAutowired();

			builder.Register(c => new SetPropertyHandler())
				.Keyed<CommandHandler>("setp")
				.PropertiesAutowired();

			builder.Register(c => new GetPropertyHandler())
				.Keyed<CommandHandler>("getp")
				.PropertiesAutowired();

			builder.Register(c => new RemovePropertyHandler())
				.Keyed<CommandHandler>("rmp")
				.PropertiesAutowired();

			builder.Register(c => new ListHandler())
				.Keyed<CommandHandler>("ls")
				.PropertiesAutowired();
			builder.Register(c => new CompilerHandler())
				.Keyed<CommandHandler>("c")
				.PropertiesAutowired();

			builder.Register(c => new SyncHandler())
				.Keyed<CommandHandler>("sync")
				.PropertiesAutowired();
			
			return builder.Build();
		}
	}
}

