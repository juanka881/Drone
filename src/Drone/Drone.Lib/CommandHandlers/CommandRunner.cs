using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Drone.Lib.Configs;
using Drone.Lib.Core;
using Drone.Lib.Exceptions;
using NLog;

namespace Drone.Lib.CommandHandlers
{
	public class CommandRunner
	{
		private readonly IContainer container;
		private readonly Logger log;

		public CommandRunner()
		{
			this.container = this.GetContainer();
			this.log = this.container.Resolve<Logger>();
		}

		private CommandTokens GetTokens(string commandString)
		{
			var tokenizer = new StringTokenizer();
			var tokens = tokenizer.GetTokens(commandString);
			return new CommandTokens(tokens);
		}

		private CommandHandler GetHandler(CommandTokens tokens)
		{
			var request = tokens.Pop();

			if (request == "drone")
				request = tokens.Pop();

			var handler = null as object;

			if (string.IsNullOrWhiteSpace(request))
				request = "help";

			if (!this.container.TryResolveKeyed(request, typeof(CommandHandler), out handler))
				throw CommandHandlerNotFoundException.Get(request);

			return (CommandHandler)handler;
		}

		private string GetFlag(CommandTokens tokens, string flag, string def)
		{
			var config = (from flagToken in tokens
						  where flagToken.Value == flag
						  let valueToken = tokens.GetAt(flagToken.Key + 1)
						  where valueToken.Key != -1
						  select new
						  {
							  flagToken,
							  valueToken
						  }).FirstOrDefault();

			if (config != null)
			{
				return config.valueToken.Value;
			}
			else
			{
				return def;
			}
		}

		private void SetFlags(CommandHandler handler, CommandTokens tokens)
		{
			handler.Flags.Filename = this.GetFlag(tokens, "-f", DroneConfig.DefaultFilename);
		}

		private void InjectServices(CommandHandler handler)
		{
			handler.Log = this.log;
			handler.Repo = this.container.Resolve<DroneConfigRepo>();
		}

		public void Run(string commandString)
		{
			try
			{
				var tokens = this.GetTokens(commandString);
				var handler = this.GetHandler(tokens);

				this.InjectServices(handler);
				this.SetFlags(handler, tokens);

				handler.Handle(tokens);	
			}
			catch (Exception ex)
			{
				this.log.Error(ex);
			}
		}

		private IContainer GetContainer()
		{
			var builder = new ContainerBuilder();

			// core services
			builder.Register(c => LogManager.GetLogger(Logging.LogName)).AsSelf();
			builder.Register(c => new DroneConfigRepo()).AsSelf();

			// handlers
			builder.Register(c => new InitHandler()).Keyed<CommandHandler>("init");
			builder.Register(c => new HelpHandler()).Keyed<CommandHandler>("help");
			builder.Register(c => new AddHandler()).Keyed<CommandHandler>("add");
			builder.Register(c => new RemoveHandler()).Keyed<CommandHandler>("rm");
			builder.Register(c => new RunHandler()).Keyed<CommandHandler>("r");
			builder.Register(c => new SetHandler()).Keyed<CommandHandler>("set");
			builder.Register(c => new ListHandler()).Keyed<CommandHandler>("ls");
			builder.Register(c => new CompilerHandler()).Keyed<CommandHandler>("c");
			builder.Register(c => new ExplainHandler()).Keyed<CommandHandler>("ex");
			builder.Register(c => new SyncHandler()).Keyed<CommandHandler>("sync");
			
			return builder.Build();
		}
	}
}

