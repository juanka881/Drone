using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Drone.Lib.Core
{
	public class RequestTokens : IEnumerable<KeyValuePair<int, string>>
	{
		private readonly IList<string> tokens;

		public RequestTokens(IEnumerable<string> tokens)
		{
			if (tokens == null)
				throw new ArgumentNullException("tokens");

			this.tokens = new List<string>(tokens);
		}

		public int Count
		{
			get
			{
				return this.tokens.Count;
			}
		}

		public string Pop()
		{
			if (this.tokens.Count == 0)
				return string.Empty;

			var str = this.tokens[0];
			this.tokens.RemoveAt(0);
			return str;
		}

		public void RemoveAt(int i)
		{
			if(i > 0 && i < this.tokens.Count)
				this.tokens.RemoveAt(i);
		}

		public KeyValuePair<int, string> GetAt(int i)
		{
			if (i > 0 && i < this.tokens.Count)
				return new KeyValuePair<int, string>(i, this.tokens[i]);
			else
				return new KeyValuePair<int, string>(-1, string.Empty);
		}

		public IEnumerator<KeyValuePair<int, string>> GetEnumerator()
		{
			return this.tokens.Select((t, i) => new KeyValuePair<int, string>(i, t)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}