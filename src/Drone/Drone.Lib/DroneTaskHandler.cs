using System;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib
{
	public abstract class DroneTaskHandler
	{
		public abstract void Handle(DroneTaskContext context);

		public static IDictionary<Type, Type> GetHandlerTypes()
		{
			var taskType = typeof(DroneTask);
			var handlerType = typeof(DroneTaskHandler);
			var genericHandleType = typeof(DroneTaskHandler<>);

			var handlerTypes = from asm in AppDomain.CurrentDomain.GetAssemblies()
							   from type in asm.GetTypes()
							   where handlerType.IsAssignableFrom(type) && 
							   (type.IsGenericType ? type.GetGenericTypeDefinition() != genericHandleType : true) &&
							   type != handlerType
							   select type;

			var types = from type in handlerTypes
						where type.BaseType.IsGenericType
						let args = type.BaseType.GetGenericArguments()
						where args != null &&
							  args.Length >= 1 &&
							  taskType.IsAssignableFrom(args[0])
						select new {TaskType = args[0], HandlerType = type};

			var dict = types.ToDictionary(x => x.TaskType, x => x.HandlerType);
			return dict;
		}
	}

	public abstract class DroneTaskHandler<T> : DroneTaskHandler where T : DroneTask
	{
		public abstract void Handle(T task, DroneTaskContext context);

		public sealed override void Handle(DroneTaskContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");

			var typedTask = context.Task as T;

			if(typedTask == null)
				throw new ArgumentException("task");

			this.Handle(typedTask, context);
		}
	}
}