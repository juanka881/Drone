using System;
using System.Collections.Generic;
using System.Linq;
using Drone.App.Core;
using Drone.Lib.Core;
using Drone.Lib.Helpers;
using Newtonsoft.Json.Linq;

namespace Drone.App.CommandHandlers
{
	public class SetPropertyHandler : CommandHandler
	{
		public override void Handle(StringTokenSet tokens)
		{
			var config = this.LoadConfig();

			if (tokens.Count == 0)
			{
				this.Log.Error("no key provided, must provide a key and value");
				return;
			}

			var key = tokens.TryGetAt(0);
			tokens.RemoveAt(0);

			if(key == null)
			{
				this.Log.Error("no key provided. please provide a key");
				return;
			}

			var type = tokens.GetFlagValueAndRemove("-t", "auto");

			var val = tokens.TryGetAt(0);
			tokens.RemoveAt(0);

			if(val == null)
			{
				this.Log.Error("no value provided. please provide a value");
				return;
			}

			config.Properties[key.Value] = (JToken)this.GetValue(val, type);

			this.SaveConfig(config);

			this.Log.Info("key: {0}", key.Value);
			this.Log.Info("value: {0}", config.Properties[key.Value]);
		}

		private object GetValue(StringToken token, string type)
		{
			var result = null as object;

			switch(type)
			{
				case "auto":
					result = this.GetValue(token, this.IdentifyStringType(token));
					break;

				case "obj":
					result = JObject.Parse(token.Value);
					break;

				case "list":
					result = JArray.Parse(token.Value);
					break;

				case "str":
				case "num":
				case "bool":
					result = new JValue(token.Value);
					break;

				default:
					throw new InvalidOperationException("invalid type provided to set command");
			}

			return result;
		}

		private string IdentifyStringType(StringToken token)
		{
			if(token.Type == StringTokenType.String)
				return "str";

			if(token.Type == StringTokenType.Json)
				return "obj";

			if(token.Type == StringTokenType.Symbol)
			{
				if(this.IsBool(token.Value))
					return "bool";

				if(this.IsNumber(token.Value))
					return "num";

				return "str";
			}

			throw new InvalidOperationException("unable to determine type for token");
		}

		private bool IsBool(string val)
		{
			var b = false;
			return bool.TryParse(val, out b);
		}

		private bool IsNumber(string val)
		{
			var n = 0;

			if(int.TryParse(val, out n))
				return true;

			var d = 0.0;

			if(double.TryParse(val, out d))
				return true;

			var l = 0L;

			if(long.TryParse(val, out l))
				return true;

			return false;
		}
	}
}