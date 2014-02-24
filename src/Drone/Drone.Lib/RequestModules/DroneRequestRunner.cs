using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Drone.Lib.Exceptions;
using Drone.Lib.Helpers;
using Drone.Lib.Repo;
using NLog;

namespace Drone.Lib.RequestModules
{
	public class DroneRequestRunner
	{
		private readonly IContainer container;
		private readonly Logger log;

		public DroneRequestRunner()
		{
			this.container = this.GetContainer();
			this.log = this.container.Resolve<Logger>();
		}

		private void ShowHelp()
		{
			this.log.Info("!help goes here");
		}

		public void Run(string requestString)
		{
			var tokenizer = new RequestTokenizer();
			var tokens = tokenizer.GetTokens(requestString).Skip(1).ToList();

			if (tokens.Count == 0)
			{
				this.ShowHelp();
				return;
			}

			var request = tokens.FirstOrDefault();
			tokens.RemoveAt(0);

			var parser = this.GetRequestParser(request);

			var parameter = parser.GetParameter(tokens);

			if(parameter == null)
				throw new NullReferenceException("request parser returned a null parameter");

			var handler = this.GetHandlerForParameter(parameter);

			this.InjectRepo(handler);
			this.InjectLogger(handler);

			this.SetParameterDronefilename(parameter, Dronefile.DefaultFilename);

			handler.Handle(parameter);
		}

		private void SetParameterDronefilename(object parameter, string filename)
		{
			if (parameter is RequestParameter)
			{
				parameter.CastTo<RequestParameter>().DroneFilename = filename;
			}
		}

		private void InjectRepo(IRequestHandler handler)
		{
			if (handler is IDronefileRepoAware)
			{
				handler.CastTo<IDronefileRepoAware>().Repo = this.container.Resolve<DronefileRepo>();
			}
		}

		public void InjectLogger(object handler)
		{
			if (handler is ILoggerAware)
			{
				handler.CastTo<ILoggerAware>().Log = this.container.Resolve<Logger>();
			}
		}

		private IRequestParser GetRequestParser(string request)
		{
			if (string.IsNullOrWhiteSpace(request))
				throw new ArgumentException("request is empty. request is expected to have a non-empty string value");

			var parser = null as object;

			if (!this.container.TryResolveNamed(request, typeof (IRequestParser), out parser))
				throw RequestParserNotFoundException.Get(request);

			return parser as IRequestParser;
		}

		private IRequestHandler GetHandlerForParameter(object parameter)
		{
			if (parameter == null)
				throw new ArgumentNullException("parameter");

			var handler = null as object;

			if (!this.container.TryResolveKeyed(parameter.GetType(), typeof(IRequestHandler), out handler))
				throw RequestHandlerNotFoundException.Get(parameter);

			return handler as IRequestHandler;
		}

		private IContainer GetContainer()
		{
			var builder = new ContainerBuilder();

			// core services
			builder.Register(c => LogManager.GetLogger(DroneLogs.LogName)).AsSelf();
			builder.Register(c => new DronefileRepo()).AsSelf();

			// parsers
			builder.Register(c => new FileModule.FileRequestParser()).Named<IRequestParser>("f");
			builder.Register(c => new RunModule.RunRequestParser()).Named<IRequestParser>("r");

			// request handlers
			// file
			builder.RegisterType<FileModule.NewHandler>().Keyed<IRequestHandler>(typeof(FileModule.NewParameter));
			builder.RegisterType<FileModule.AddHandler>().Keyed<IRequestHandler>(typeof(FileModule.AddParameter));
			builder.RegisterType<FileModule.RemoveHandler>().Keyed<IRequestHandler>(typeof(FileModule.RemoveParameter));

			// runner
			builder.RegisterType<RunModule.RunRequestHandler>().Keyed<IRequestHandler>(typeof(RunModule.RunParameter));
			
			return builder.Build();
		}
	}
}
