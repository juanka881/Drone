using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Drone.Lib.Compilers
{
	public class MSBuildHelper
	{
		public static readonly string OutputPattern = @"^(?<origin>.*)\:\s+(?<subcategory>.*)(?<category>error|warning)\s+(?<code>[a-zA-Z]+\d+)\:(?<text>.*)$";
		public static readonly Regex OutputRegex = new Regex(OutputPattern, RegexOptions.Compiled | RegexOptions.Singleline);

		public static MSBuildOutputLevel GetOutputLevel(string data)
		{
			if (string.IsNullOrWhiteSpace(data))
				return MSBuildOutputLevel.Normal;

			var match = OutputRegex.Match(data);

			if(!match.Success)
				return MSBuildOutputLevel.Normal;

			var category = match.Groups["error"].Value;

			if(category == "warning")
				return MSBuildOutputLevel.Warning;
			else if(category == "error")
				return MSBuildOutputLevel.Error;
			else
				return MSBuildOutputLevel.Normal;
		}
	}
}