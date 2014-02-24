using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Drone.Lib.Exceptions;
using Drone.Lib.Repo;
using NLog;

namespace Drone.Lib.RequestModules
{
	public abstract class RequestHandler<T> : IDronefileRepoAware, ILoggerAware, IRequestHandler where T : class 
	{
		public DronefileRepo Repo { get; set; }
		public Logger Log { get; set; }

		public void Handle(object parameter)
		{
			var typedParameter = parameter as T;

			if (typedParameter == null)
				throw RequestHandlerParameterCastException.Get(parameter, typeof(T));

			if(this.Repo == null)
				throw new NullReferenceException("Handler Repo is null");

			if(this.Log == null)
				throw new NullReferenceException("Handler Log is null");

			this.HandleCore(typedParameter);
		}

		public abstract void HandleCore(T parameter);
	}
}