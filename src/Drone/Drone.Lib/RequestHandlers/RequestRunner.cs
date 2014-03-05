using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Drone.Lib.Configs;
using Drone.Lib.Core;
using Drone.Lib.Exceptions;
using NLog;

namespace Drone.Lib.RequestHandlers
{
	public class RequestRunner
	{
		private readonly IContainer container;
		private readonly Logger log;

		public RequestRunner()
		{
			this.container = this.GetContainer();
			this.log = this.container.Resolve<Logger>();
		}

		private RequestTokens GetRequestTokens(string requestString)
		{
			var tokenizer = new StringTokenizer();
			var tokens = tokenizer.GetTokens(requestString);
			return new RequestTokens(tokens);
		}

		private RequestHandler GetRequestHandler(RequestTokens tokens)
		{
			var request = tokens.Pop();

			if (request == "drone")
				request = tokens.Pop();

			var handler = null as object;

			if (string.IsNullOrWhiteSpace(request))
				request = "help";

			if (!this.container.TryResolveKeyed(request, typeof(RequestHandler), out handler))
				throw RequestHandlerNotFoundException.Get(request);

			return (RequestHandler)handler;
		}

		private string GetFlag(RequestTokens tokens, string flag, string def)
		{
			var config = (from flagToken in tokens
						  where flagToken.Value == "-f"
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

		public void Run(string requestString)
		{
			var tokens = this.GetRequestTokens(requestString);
			var handler = this.GetRequestHandler(tokens);

			handler.Log = this.log;
			handler.Repo = this.container.Resolve<DroneConfigRepo>();

			handler.Flags.Filename = this.GetFlag(tokens, "-f", DroneConfig.DefaultFilename);

			try
			{
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
			builder.Register(c => new InitHandler()).Keyed<RequestHandler>("init");
			builder.Register(c => new HelpHandler()).Keyed<RequestHandler>("help");
			builder.Register(c => new AddHandler()).Keyed<RequestHandler>("add");
			builder.Register(c => new RemoveHandler()).Keyed<RequestHandler>("rm");
			builder.Register(c => new RunHandler()).Keyed<RequestHandler>("r");
			builder.Register(c => new SetHandler()).Keyed<RequestHandler>("set");
			builder.Register(c => new ListHandler()).Keyed<RequestHandler>("ls");
			builder.Register(c => new CompilerHandler()).Keyed<RequestHandler>("c");
			builder.Register(c => new ExplainHandler()).Keyed<RequestHandler>("ex");
			builder.Register(c => new SyncHandler()).Keyed<RequestHandler>("sync");
			
			return builder.Build();
		}
	}
}

