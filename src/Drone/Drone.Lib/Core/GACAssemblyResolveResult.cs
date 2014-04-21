using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Drone.Lib.Core
{
	public class GACAssemblyResolveResult
	{
		public string AssemblyPath { get; private set; }

		public bool IsSuccess
		{
			get
			{
				return this.Exception == null;
			}
		}

		public Win32Exception Exception { get; private set; }

		public GACAssemblyResolveResult(string path)
		{
			this.AssemblyPath = path;
		}

		public GACAssemblyResolveResult(Win32Exception ex)
		{
			this.Exception = ex;
		}
	}
}