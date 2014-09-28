using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Drone.Lib.Helpers
{
	public class ParameterTokenizer
	{
		public IEnumerable<ParameterToken> GetTokens(string str)
		{
			if (string.IsNullOrWhiteSpace(str))
				yield break;

			var currentIndex = 0;

			var sb = new StringBuilder();
			var stack = new Stack<char>();
			var counter = 0;

			while (currentIndex != -1)
			{
				var startIndex = this.TryGetIndexOfNonWhitespace(str, currentIndex);

				if (startIndex == -1)
					yield break;

				var c = str[startIndex];
				var endIndex = -1;
				var type = ParameterTokenType.Symbol;

				if (this.IsObjectStart(c))
				{
					type = ParameterTokenType.Json;
					endIndex = this.ReadObjectIntoBuffer(str, startIndex, sb, stack);
				}
				else if (this.IsStringStart(c))
				{
					type = ParameterTokenType.String;
					endIndex = this.ReadStringIntoBuffer(str, startIndex, sb, stack, false);
				}
				else
				{
					type = ParameterTokenType.Symbol;
					endIndex = this.ReadSymbolIntoBuffer(str, startIndex, sb);
				}

				var token = sb.ToString();

				yield return new ParameterToken(counter, token, type);

				sb.Clear();
				stack.Clear();
				currentIndex = endIndex;
				counter += 1;
			}
		}

		public int ReadSymbolIntoBuffer(string str, int startIndex, StringBuilder sb)
		{
			var i = 0;

			for (i = startIndex; i < str.Length; i++)
			{
				var c = str[i];

				if (char.IsWhiteSpace(c))
				{
					return i;
				}
				else
				{
					sb.Append(c);
				}
			}

			return i;
		}

		private int ReadStringIntoBuffer(string str, int startIndex, StringBuilder sb, Stack<char> stack, bool includeQuotes)
		{
			var i = 0;
			var stackInitialCount = stack.Count;

			for (i = startIndex; i < str.Length; i++)
			{
				var c = str[i];

				if (this.IsStringStart(c) && i == startIndex)
				{
					if (includeQuotes)
						sb.Append(c);

					stack.Push(c);

					continue;
				}

				// handle \" cases
				if (c == '\\' && this.TryGetChar(str, i + 1) == '\"')
				{
					i += 1;
					sb.Append('\"');
					continue;
				}

				if (this.IsStringEnd(c) && stack.Count > 0 && stack.Peek() == c)
				{
					if (includeQuotes)
						sb.Append(c);

					stack.Pop();

					continue;
				}

				if (stack.Count == stackInitialCount)
				{
					return i;
				}
				else
				{
					sb.Append(c);	
				}
			}

			if (stack.Count != stackInitialCount)
			{
				throw new Exception(string.Format("unable to get find closing quote for string, expected: {0}", stack.Peek()));
			}

			return i;
		}

		private int ReadObjectIntoBuffer(string str, int startIndex, StringBuilder sb, Stack<char> stack)
		{
			var i = 0;
			var stackInitialCount = stack.Count;

			for (i = startIndex; i < str.Length; i++)
			{
				var c = str[i];

				if (this.IsObjectStart(c))
				{
					stack.Push(c);
				}

				if (this.IsObjectEnd(c))
				{
					stack.Pop();
				}

				if (this.IsStringStart(c))
				{
					i = this.ReadStringIntoBuffer(str, i, sb, stack, true) - 1;
					continue;
				}

				sb.Append(c);

				if (stack.Count == stackInitialCount)
				{
					return i + 1;
				}
			}

			if (stack.Count != stackInitialCount)
			{
				throw new Exception(string.Format("unable to get find closing quote for string, expected: {0}", stack.Peek()));
			}

			return i;
		}

		private int TryGetIndexCore(string str, int startIndex, Func<char, bool> predicate)
		{
			if (string.IsNullOrEmpty(str))
				return -1;

			if (startIndex > str.Length - 1)
				return -1;

			for (var i = startIndex; i < str.Length; i++)
			{
				if (predicate(str[i]))
					return i;
			}

			return -1;
		}

		private int TryGetIndexOfNonWhitespace(string str, int startIndex)
		{
			return this.TryGetIndexCore(str, startIndex, c => !char.IsWhiteSpace(c));
		}

		private bool IsStringStart(char c)
		{
			return c == '\'' || c == '\"';
		}

		private bool IsStringEnd(char c)
		{
			return this.IsStringStart(c);
		}

		private bool IsObjectStart(char c)
		{
			return c == '{';
		}

		private bool IsObjectEnd(char c)
		{
			return c == '}';
		}

		private char? TryGetChar(string str, int index)
		{
			if (index < 0 || index > str.Length - 1)
				return null;

			return str[index];
		}
	}
}
