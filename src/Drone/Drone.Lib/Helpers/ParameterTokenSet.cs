using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public class ParameterTokenSet : IEnumerable<ParameterToken>
	{
		private readonly IList<ParameterToken> tokens;

		public ParameterTokenSet(IEnumerable<ParameterToken> tokens)
		{
			if (tokens == null)
				throw new ArgumentNullException("tokens");

			this.tokens = new List<ParameterToken>(tokens);
		}

		public int Count
		{
			get
			{
				return this.tokens.Count;
			}
		}

		public ParameterToken Pop()
		{
			if (this.tokens.Count == 0)
				return null;

			var token = this.tokens[0];
			this.RemoveAt(0);
			return token;
		}

		public void RemoveAt(int index)
		{
			if(index >= 0 && index < this.tokens.Count && this.tokens.Count > 0)
			{
				this.tokens.RemoveAt(index);

				for(var i = 0; i < this.tokens.Count; i++)
					this.tokens[i].Index = i;
			}
		}

		public ParameterToken TryGetAt(int i)
		{
			if(i >= 0 && i < this.tokens.Count)
				return this.tokens[i];
			else
				return null;
		}

		public bool PopFlag(string flag)
		{
			if(string.IsNullOrWhiteSpace(flag))
				throw new ArgumentException("flag is empty or null", "name");

			var token = this.tokens.FirstOrDefault(x => x.Value == flag);

			if (token != null)
				this.tokens.RemoveAt(token.Index);

			return token != null;
		}

		public string PopFlagValue(string flag, string def)
		{
			var pair = (from flagToken in this.tokens
						where flagToken.Value == flag
						let valueToken = this.TryGetAt(flagToken.Index + 1)
						where valueToken != null
						select new { flagToken, valueToken }).FirstOrDefault();

			if (pair != null)
			{
				this.RemoveAt(pair.flagToken.Index);
				this.RemoveAt(pair.flagToken.Index);
				return pair.valueToken.Value;
			}
			else
			{
				return def;
			}
		}

		public IEnumerator<ParameterToken> GetEnumerator()
		{
			return this.tokens.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}