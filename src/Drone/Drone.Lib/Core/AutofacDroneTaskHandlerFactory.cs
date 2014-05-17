using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Drone.Lib.Core
{
	public class AutofacDroneTaskHandlerFactory : DroneTaskHandlerFactory
	{
		private readonly ILifetimeScope rootScope;
		private ILifetimeScope scope;

		public AutofacDroneTaskHandlerFactory(ILifetimeScope rootScope)
		{
			if(rootScope == null)
				throw new ArgumentNullException("rootScope");

			this.rootScope = rootScope;
		}

		public override DroneTaskHandler TryGetHandler(DroneTask task)
		{
			if(task == null)
				throw new ArgumentNullException("task");

			if(this.scope == null)
				this.Init();

			var handler = null as object;
			this.scope.TryResolveKeyed(task.GetType(), typeof(DroneTaskHandler), out handler);
			return handler as DroneTaskHandler;
		}

		private void Init()
		{
			if(this.scope != null)
				return;

			this.scope = this.rootScope.BeginLifetimeScope(x =>
			{
				var handlerTypes = DroneTaskHandler.GetHanlderTypes();

				foreach(var item in handlerTypes)
					x.RegisterType(item.Value).Keyed<DroneTaskHandler>(item.Key);
			});
		}
	}
}