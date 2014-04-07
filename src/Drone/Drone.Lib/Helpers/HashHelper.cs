using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Drone.Lib.Helpers
{
	public class HashHelper
	{
		public static string GetHash(string data)
		{
			var dataBytes = Encoding.UTF8.GetBytes(data);
			using (var provider = new SHA1CryptoServiceProvider())
			{
				var hashBytes = provider.ComputeHash(dataBytes);
				var result = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
				return result;
			}
		}
	}
}