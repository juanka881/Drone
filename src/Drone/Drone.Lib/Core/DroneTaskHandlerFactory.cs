using System;
using System.Collections.Generic;
using System.Linq;
using Drone.Lib.Helpers;

namespace Drone.Lib.Core
{
	public class DroneTaskHandlerFactory
	{
		private Option<IDictionary<Type, Type>> handlerTypes;

		public DroneTaskHandlerFactory()
		{
			this.handlerTypes = Option.None<IDictionary<Type, Type>>();
		}

		public virtual Option<DroneTaskHandler> TryGetHandler(DroneTask task)
		{
			if (task == null)
				throw new ArgumentNullException("task");

			if (!this.handlerTypes.HasValue)
			{
				this.handlerTypes = Option.From(DroneTaskHandler.GetHandlerTypes());

				if (!this.handlerTypes.HasValue)
					throw DroneTaskHandlerNotFoundException.Get(task);
			}

			var handlerType = null as Type;
			this.handlerTypes.Value.TryGetValue(task.GetType(), out handlerType);

			if (handlerType == null)
				throw DroneTaskHandlerNotFoundException.Get(task);

			var handler = Activator.CreateInstance(handlerType) as DroneTaskHandler;
			return handler;
		}
	}
}