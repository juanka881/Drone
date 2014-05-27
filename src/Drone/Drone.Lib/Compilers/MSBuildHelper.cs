using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Drone.Lib.Compilers
{
	public class MSBuildHelper
	{
		public static readonly string NormalErrorPattern = @"(?<file>.*)\((?<line>\d+),(?<column>\d+)\):\s+(?<error>\w+)\s+(?<number>[\d\w]+):\s+(?<message>.*)";
		public static readonly string GeneralErrorPattern = @"(?<error>.+?)\s+(?<number>[\d\w]+?):\s+(?<message>.*)";

		public static readonly Regex NormalErrorRegex = new Regex(NormalErrorPattern, RegexOptions.Compiled);
		public static readonly Regex GeneralErrorRegex = new Regex(GeneralErrorPattern, RegexOptions.Compiled);

		public static MSBuildOutputLevel GetOutputLevel(string data)
		{
			if (string.IsNullOrWhiteSpace(data))
				return MSBuildOutputLevel.Normal;

			var match = NormalErrorRegex.Match(data);

			if (!match.Success)
			{
				match = GeneralErrorRegex.Match(data);
			}

			if (!match.Success)
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