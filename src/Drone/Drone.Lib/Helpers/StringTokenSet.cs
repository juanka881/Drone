using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Helpers
{
	public class StringTokenSet : IEnumerable<StringToken>
	{
		private readonly IList<StringToken> tokens;

		public StringTokenSet(IEnumerable<StringToken> tokens)
		{
			if (tokens == null)
				throw new ArgumentNullException("tokens");

			this.tokens = new List<StringToken>(tokens);
		}

		public int Count
		{
			get
			{
				return this.tokens.Count;
			}
		}

		public StringToken Pop()
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

		public StringToken TryGetAt(int i)
		{
			if(i >= 0 && i < this.tokens.Count)
				return this.tokens[i];
			else
				return null;
		}

		public bool GetFlagAndRemove(string flag)
		{
			if(string.IsNullOrWhiteSpace(flag))
				throw new ArgumentException("flag is empty or null", "flag");

			var token = this.tokens.FirstOrDefault(x => x.Value == flag);

			if (token != null)
				this.tokens.RemoveAt(token.Index);

			return token != null;
		}

		public string GetFlagValueAndRemove(string flag, string def)
		{
			var pair = (from flagToken in this.tokens
						where flagToken.Value == flag
						let valueToken = this.TryGetAt(flagToken.Index + 1)
						where valueToken != null
						select new
						{
							flagToken,
							valueToken
						}).FirstOrDefault();

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

		public IEnumerator<StringToken> GetEnumerator()
		{
			return this.tokens.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}