using Drone.Lib;
using Drone.Lib.Core;
using Drone.Lib.Helpers;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Drone.App
{
	public class DroneApp
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

		private readonly DroneService droneService;
		private readonly Logger log;

		public DroneApp()
		{
			this.droneService = new DroneService();
			this.log = DroneLogManager.GetLog();
		}

		private ParameterTokenSet GetTokens(string commandLine)
		{
			var tokenizer = new ParameterTokenizer();
			var tokens = tokenizer.GetTokens(commandLine);
			return new ParameterTokenSet(tokens);
		}

		private DroneFlags GetFlags(ParameterTokenSet tokens)
		{
			var configFilename = tokens.PopFlagValue("-f", DroneConfig.DroneFileName);
			var isDebugEnabled = tokens.PopFlag("-d");
			var isConsoleLogColorsEnabled = !tokens.PopFlag("-no-colors");
			var logLevel = DroneLogManager.GetLogLevelFromString(tokens.PopFlagValue("-l", "info"));

			var flags = new DroneFlags(
				configFilename,
				isDebugEnabled,
				isConsoleLogColorsEnabled,
				logLevel);

			return flags;
		}

		private void ApplyLogConfig(DroneFlags flags)
		{
			if (flags == null)
				throw new ArgumentNullException("flags");

			var rules = (from rule in LogManager.Configuration.LoggingRules
						 where rule.LoggerNamePattern == "drone" || rule.LoggerNamePattern == "drone.task.*"
						 select rule).ToList();

			foreach (var rule in rules)
			{
				var level = flags.LogLevel;

				if (rule.LoggerNamePattern == "drone" && level > LogLevel.Info)
					level = LogLevel.Info;

				var effectiveLevels = logLevels.Where(l => l >= level).ToList();

				this.ApplyLogLevel(rule, effectiveLevels);
			}

			if (!flags.IsConsoleLogColorsEnabled)
				this.DisableConsoleLogColors(rules);

			var errorFileTarget = (from target in LogManager.Configuration.AllTargets
								   where target.Name == "drone.error.file" && target is FileTarget
								   select target as FileTarget).FirstOrDefault();

			if (errorFileTarget != null)
			{
				var configPath = Path.GetFullPath(flags.ConfigFileName);
				var configDir = Path.GetDirectoryName(configPath);
				errorFileTarget.FileName = Path.Combine(configDir, "drone.errors.txt");
			}

			LogManager.ReconfigExistingLoggers();
		}

		private void ApplyLogLevel(LoggingRule rule, IList<LogLevel> levels)
		{
			foreach (var level in rule.Levels)
				rule.DisableLoggingForLevel(level);

			foreach (var level in levels)
				rule.EnableLoggingForLevel(level);
		}

		private void DisableConsoleLogColors(IList<LoggingRule> rules)
		{
			foreach (var rule in rules)
			{
				foreach (var target in rule.Targets.OfType<ColoredConsoleTarget>())
				{
					target.UseDefaultRowHighlightingRules = false;
					target.RowHighlightingRules.Clear();
					target.WordHighlightingRules.Clear();
				}
			}
		}

		private object GetTokenJsonValue(ParameterToken token, string type)
		{
			var result = null as object;

			switch (type)
			{
				case "auto":
					result = this.GetTokenJsonValue(token, this.GetTokenJsonType(token));
					break;

				case "obj":
					result = JObject.Parse(token.Value);
					break;

				case "list":
					result = JArray.Parse(token.Value);
					break;

				case "str":
				case "num":
				case "bool":
					result = new JValue(token.Value);
					break;

				default:
					throw new InvalidOperationException("invalid type provided to set command");
			}

			return result;
		}

		private string GetTokenJsonType(ParameterToken token)
		{
			if (token.Type == ParameterTokenType.String)
				return "str";

			if (token.Type == ParameterTokenType.Json)
				return "obj";

			if (token.Type == ParameterTokenType.Symbol)
			{
				if (this.IsBool(token.Value))
					return "bool";

				if (this.IsNumber(token.Value))
					return "num";

				return "str";
			}

			throw new InvalidOperationException("unable to determine type for token");
		}

		private bool IsBool(string val)
		{
			var b = false;
			return bool.TryParse(val, out b);
		}

		private bool IsNumber(string val)
		{
			var n = 0;

			if (int.TryParse(val, out n))
				return true;

			var d = 0.0;

			if (double.TryParse(val, out d))
				return true;

			var l = 0L;

			if (long.TryParse(val, out l))
				return true;

			return false;
		}

		private string GetTaskListPositionSymbol(int pos, int max, bool secondLevel)
		{
			if(pos == 0 && !secondLevel)
			{
				return "┌";
			}
			else if(pos == max)
			{
				return "└";
			}
			else
			{
				return "├";
			}
		}

		private string GetCommand(ParameterTokenSet tokens, string[] args)
		{
			if (tokens == null)
				throw new ArgumentNullException("tokens");

			var commandToken = tokens.Pop();
			var command = "help";

			if (commandToken != null && args != null && args.Length >= 1 && commandToken.Value == args[0])
				commandToken = tokens.Pop();

			if (commandToken != null)
				command = commandToken.Value;

			return command;
		}

		private void HandleCommand(string command, ParameterTokenSet tokens, DroneFlags flags)
		{
			if (string.IsNullOrWhiteSpace(command))
				throw new ArgumentException("command is empty or null", "command");

			if(flags == null)
				throw new ArgumentNullException("flags");

			var action = null as Action<ParameterTokenSet, DroneFlags>;

			switch (command.ToLower())
			{
				case "init":
					action = this.InitCommand;
					break;

				case "help":
					action = this.ShowHelpCommand;
					break;

				case "add":
					action = this.AddCommand;
					break;

				case "rm":
					action = this.RemoveCommand;
					break;

				case "r":
					action = this.RunCommand;
					break;

				case "sp":
					action = this.SetPropertyCommand;
					break;

				case "gp":
					action = this.GetPropertyCommand;
					break;

				case "rp":
					action = this.RemovePropertyCommand;
					break;

				case "ls":
					action = this.ListCommand;
					break;

				case "c":
					action = this.CompileCommand;
					break;

				case "sync":
					action = this.SyncCommand;
					break;
			}

			if (action == null)
			{
				throw UnknownCommandException.Get(command);
			}
			else
			{
				action(tokens, flags);
			}
		}

		private void SyncCommand(ParameterTokenSet tokens, DroneFlags flags)
		{
			throw new Exception("not implemented exception");
		}

		private void CompileCommand(ParameterTokenSet tokens, DroneFlags flags)
		{
			var config = this.droneService.LoadConfig(flags.ConfigFileName);
			this.droneService.CompileTasks(config, LogLevel.Info);
		}

		private void ListCommand(ParameterTokenSet tokens, DroneFlags flags)
		{
			var config = this.droneService.LoadConfig(flags.ConfigFileName);

			// need to add pattern matching to the check
			var tasks = this.droneService.GetTasks(config, flags, string.Empty).ToList(); 
			
			var taskCounter = 0;

			foreach (var task in tasks)
			{
				this.log.Info("{0}─ {1}", this.GetTaskListPositionSymbol(taskCounter, tasks.Count - 1, false), task.Name);

				if (task.Dependencies.Count > 0)
				{
					var depCounter = 0;

					foreach (var dep in task.Dependencies)
					{
						this.log.Info("│  {0}─ {1}", this.GetTaskListPositionSymbol(depCounter, task.Dependencies.Count - 1, true), dep);
						depCounter += 1;
					}
				}

				taskCounter += 1;
			}
		}

		private void RemovePropertyCommand(ParameterTokenSet tokens, DroneFlags flags)
		{
			var config = this.droneService.LoadConfig(flags.ConfigFileName);
			var key = tokens.TryGetAt(0);

			if (key != null)
			{
				if (config.Props.Remove(key.Value))
					this.log.Info("property '{0}' removed", key.Value);
			}

			this.droneService.SaveConfig(config);
		}

		private void GetPropertyCommand(ParameterTokenSet tokens, DroneFlags flags)
		{
			var config = this.droneService.LoadConfig(flags.ConfigFileName);

			var key = tokens.TryGetAt(0);

			if (key == null)
			{
				foreach (var prop in config.Props)
				{
					this.log.Info("{0}: {1}", prop.Key, prop.Value);
				}
			}
			else
			{
				var token = null as JToken;
				if (!config.Props.TryGetValue(key.Value, out token))
					return;

				this.log.Info("{0}: {1}", key.Value, token);
			}
		}

		private void SetPropertyCommand(ParameterTokenSet tokens, DroneFlags flags)
		{
			var config = this.droneService.LoadConfig(flags.ConfigFileName);

			if (tokens.Count == 0)
			{
				this.log.Error("no key provided, must provide a key and value");
				return;
			}

			var key = tokens.Pop();
			
			if (key == null)
			{
				this.log.Error("no key provided. please provide a key");
				return;
			}

			var type = tokens.PopFlagValue("-t", "auto");

			var val = tokens.Pop();

			if (val == null)
			{
				this.log.Error("no value provided. please provide a value");
				return;
			}

			config.Props[key.Value] = (JToken)this.GetTokenJsonValue(val, type);

			this.droneService.SaveConfig(config);

			this.log.Info("key: {0}", key.Value);
			this.log.Info("value: {0}", config.Props[key.Value]);
		}

		private void RunCommand(ParameterTokenSet tokens, DroneFlags flags)
		{
			var config = this.droneService.LoadConfig(flags.ConfigFileName);
			var taskNames = tokens.Where(x => !x.Value.StartsWith("-")).Select(x => x.Value);
			this.droneService.RunTasks(config, flags, taskNames);
		}

		private void RemoveCommand(ParameterTokenSet tokens, DroneFlags flags)
		{
			var files = tokens.Select(x => new { lower = x.Value.ToLower(), val = x.Value }).ToList();

			var sources = files
				.Where(x => x.lower.EndsWith(".cs"))
				.Select(x => x.val)
				.ToList();

			var refs = files
				.Where(x => x.lower.EndsWith(".dll"))
				.Select(x => x.val)
				.ToList();

			if (sources.Count == 0 && refs.Count == 0)
			{
				this.log.Warn("nothing to remove. no files specified");
				return;
			}

			var config = this.droneService.LoadConfig(flags.ConfigFileName);

			this.droneService.RemoveFiles(config, sources, refs);
			this.droneService.SaveConfig(config);
		}

		private void AddCommand(ParameterTokenSet tokens, DroneFlags flags)
		{
			var files = tokens.Select(x => new { lower = x.Value.ToLower(), val = x.Value }).ToList();

			var sources = files
				.Where(x => x.lower.EndsWith(".cs"))
				.Select(x => x.val)
				.ToList();

			var refs = files
				.Where(x => x.lower.EndsWith(".dll"))
				.Select(x => x.val)
				.ToList();

			if (sources.Count == 0 && refs.Count == 0)
			{
				this.log.Warn("nothing to add. no files specified");
				return;
			}

			var config = this.droneService.LoadConfig(flags.ConfigFileName);

			this.droneService.AddFiles(config, sources, refs);
			this.droneService.SaveConfig(config);
		}

		private void ShowHelpCommand(ParameterTokenSet tokens, DroneFlags flags)
		{
			this.log.Info("!help goes here");
		}

		private void InitCommand(ParameterTokenSet tokens, DroneFlags flags)
		{
			if (File.Exists(flags.ConfigFileName))
			{
				this.log.Warn("file '{0}' already exists", flags.ConfigFileName);
				return;
			}

			var config = new DroneConfig();
			config.SetConfigFilename(flags.ConfigFileName);

			this.droneService.InitDroneDir(config);

			this.droneService.SaveConfig(config);

			this.log.Info("created '{0}'", Path.GetFileName(config.DroneDirPath));
			this.log.Info("created '{0}'", Path.GetFileName(config.FilePath));
		}

		public void Run(string commandLine)
		{
			try
			{
				DroneLogManager.DeleteErrorLogFile();

				var tokens = this.GetTokens(commandLine);
				var flags = this.GetFlags(tokens);

				this.ApplyLogConfig(flags);

				this.log.Info(string.Empty);

				var command = this.GetCommand(tokens, Environment.GetCommandLineArgs());

				this.HandleCommand(command, tokens, flags);
			}
			catch(Exception ex)
			{
				this.log.ExceptionAndData(ex);
			}
		}
	}
}