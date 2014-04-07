using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Drone.Lib.Compilers
{
	[Serializable]
	public partial class CSharpCompilerInvocationException : Exception
	{
		public CSharpCompilerInvocationException()
		{
		}

		public CSharpCompilerInvocationException(string message) : base(message)
		{
		}

		public CSharpCompilerInvocationException(string message, Exception inner) : base(message, inner)
		{
		}

		protected CSharpCompilerInvocationException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{

		}

		public static CSharpCompilerInvocationException Get(string command, string commandArgs, Exception sex)
		{
			var ex = new CSharpCompilerInvocationException("unknown error while invoking csharp compiler", sex);
			ex.Data["command"] = command;
			ex.Data["command-args"] = commandArgs;
			return ex;
		}
	}
}