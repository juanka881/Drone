using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Drone.Lib.Core
{
	public class GACAssemblyResolver
	{
		[DllImport("fusion.dll")]
		private static extern IntPtr CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);

		[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
		Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
		private interface IAssemblyCache
		{
			int Dummy1();
			[PreserveSig]
			IntPtr QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] String assemblyName, ref AssemblyInfo assemblyInfo);
			int Dummy2();
			int Dummy3();
			int Dummy4();
		}

		[StructLayout(LayoutKind.Sequential)]
		private struct AssemblyInfo
		{
			public int cbAssemblyInfo;
			public int assemblyFlags;
			public long assemblySizeInKB;
			[MarshalAs(UnmanagedType.LPWStr)]
			public String currentAssemblyPath;
			public int cchBuf;
		}

		private IAssemblyCache assemblyCache;

		private IAssemblyCache GetAssemblyCache()
		{
			if(this.assemblyCache == null)
			{
				var cache = null as IAssemblyCache;

				var hr = CreateAssemblyCache(out cache, 0);

				if (hr == IntPtr.Zero)
				{
					this.assemblyCache = cache;
				}
				else
				{
					throw new Win32Exception();
				}
			}

			return this.assemblyCache;
		}

		public GACAssemblyResolveResult TryResolve(string assemblyName)
		{
			try
			{
				var assembyInfo = new AssemblyInfo { cchBuf = 512 };
				assembyInfo.currentAssemblyPath = new String(' ', assembyInfo.cchBuf);

				var cache = this.GetAssemblyCache();

				var hr = cache.QueryAssemblyInfo(1, assemblyName, ref assembyInfo);

				if(hr == IntPtr.Zero)
				{
					return new GACAssemblyResolveResult(assembyInfo.currentAssemblyPath);
				}
				else
				{
					throw new Win32Exception();
				}
			}
			catch(Win32Exception ex)
			{
				return new GACAssemblyResolveResult(ex);
			}
		}
	}
}
