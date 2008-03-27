using System;
using System.Collections.Generic;
using System.Text;

namespace zeroParse
{
	public class Rule : IEnumerable<Rule>
	{
		private string _Name;
		private Rule _Inner;

		public Rule()
		{
		}

		public Rule(string name)
		{
			this.Name = name;
		}

		public Rule(params Rule[] rules)
		{
			this.Inner = this.Append(null, 0, rules);
		}

		Rule Append(Rule a, int index, params Rule[] rules)
		{
			if (rules.Length > index)
			{
				if (a == null)
				{
					return this.Append(rules[index], index + 1, rules);
				}
				else
					return this.Append(a & rules[index], index + 1, rules);
			}
			else
				return a;
		}

		public Rule Inner
		{
			get { return _Inner; }
			set { _Inner = value; }
		}

		public string Name
		{
			get { return _Name ?? this.DefaultName; }
			set { _Name = value; }
		}

		protected virtual string DefaultName
		{
			get { return null; }
		}

		bool _Ignore = false;

		public bool Ignore
		{
			get { return _Ignore; }
			set { _Ignore = value; }
		}

		#region Primitive

		private bool _Primitive = false;

		public bool Primitive
		{
			get { return _Primitive; }
			set
			{
				if (_Primitive != value)
				{
					_Primitive = value;
				}
			}
		}
		#endregion Primitive

		const int MaxDepth = 2000;
		static DateTime lastOutput;
		static TimeSpan outputInterval = TimeSpan.FromSeconds(1);
		public Token Match(ParserContext context)
		{
			//try
			//{
			//Console.WriteLine(new StringBuilder().Append(' ', context.Depth).Append(this).Append("").ToString());

			context.Rule = this;
			//context.Success = true;
			if (context.Depth > MaxDepth)
			{
				Console.WriteLine("Canceling in " + this + " at depth" + context.Depth + ", line" + context.Line + ":\n\t" + context);
				//return null;
				throw new ParseFailedException(this, context, "MaxDepth reached", null);
			}

			if (context.Source == null || context.Index >= context.Source.Length)
			{
				Console.WriteLine("EOF in " + this + " at depth" + context.Depth + ", line" + context.Line + ":\n\t" + context);
				return null;
			}

#if !VERBOSE
			{
				Token result = null;
				try
				{
					result = this.MatchAll(context);
				}
				catch (ParseFailedException exc)
				{
					exc.ContextTrace.Add(context);
					throw exc;
				}
				if (result != null)
				{
					//Console.WriteLine(new StringBuilder().Append(' ', context.Depth).Append(result).Append("").ToString());
					if (context.Success)
					{
						if (context.Debug && !this.Primitive && !this.Ignore)
						{
							if (this.Name != null)
								Console.WriteLine(this.Name + ": " + context.Line + "." + context.Index + " := " + result.ToString().Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n") + " := " + context.ToString().Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n"));
							//else
							//    Console.Write((this.GetType().Name + ": " + context.Line + "." + context.Index + " := " + context.ToString().Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n")).PadRight(Console.WindowWidth - 3) + "\r");
						}
						//Token inner;
						//while ((inner = context.WhiteSpaces.Match((context.Push(result.Start + result.BlockLength)))) != null)
						//    if (inner.Length > 0)
						//        result.Append(inner);


						//context.Trim();
					}
					//context.WhiteSpaces.Match(context);
					if (DateTime.Now - lastOutput > outputInterval)
					{
						Console.Write(("Parsing " + (result.Start + result.Length).ToString().PadLeft(6) + " / " + context.Source.Length.ToString().PadRight(6) + ": " + context.ToString().Replace("\0", @"\0").Replace("\n", @"\n").Replace("\t", @"\t")).PadRight(Console.WindowWidth - 2) + "\r");
						lastOutput = DateTime.Now;
					}

					return result;
				}
				else
				{
					if (context.Debug && !this.Primitive && !this.Ignore && this.Name != null)
					{
						Console.Write((this.Name.PadRight(15) + " failed" + context.ToString().Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n")).PadRight(50) + "\r");
					}
					context.Success = false;
					return null;
				}
			}
#else
				return this.MatchAll(context);
#endif
			//}
			//catch (Exception exc)
			//{
			//    throw new ParseFailedException(this, context, exc.Message, exc);
			//}
			//finally { }
		}

		protected virtual Token MatchAll(ParserContext context)
		{
			Token result = context.Result;

			if (this.Inner != null)
			{
				Token inner = null;
				//while ((inner = this.MatchWhiteSpace(context.Push(result.Start + result.BlockLength))) != null)
				//    this.AppendToken(result, inner);

				inner = this.MatchInner(context.Push());
				if (inner != null)
				{
					result = result ?? this.CreateToken(context, 0);
					result.Append(inner);
				}
				else
				{
					context.Success = false;
				}
			}
			else
				result = this.MatchThis(context);

			if (context.Success)
				return result;
			else
				return null;
		}

		protected virtual Token MatchThis(ParserContext context)
		{
			return null;
		}

		protected virtual Token MatchInner(ParserContext context)
		{
			return this.Inner != null ? this.Inner.Match(context) : null;
		}

		//protected Token MatchWhiteSpace(ParserContext context)
		//{
		//    return context.WhiteSpaces.Match(context);
		//}

		protected Token CreateToken(ParserContext context, int length)
		{
			Token token = new Token();

			token.Rule = this;
			token.Name = this.Name;

			token.Start = context.Index;
			token.Length = length;
			token.Context = context;
			if (context != null)
			{
				context.Result = token;
				context.Success = true;

				if (context.Outer != null && context.Outer.Result != null)
					token.Outer = context.Outer.Result;
			}

			token.Value = context.Source.Substring(context.Index, length);

			this.FillToken(token);

			return token;
		}

		protected virtual void FillToken(Token token)
		{
		}

		public override string ToString()
		{
			return this.Name ?? this.Structure ?? base.ToString();
		}

		public string Structure
		{
			get { return this.DescribeStructure(new List<Rule>()); }
		}

		public virtual string DescribeStructure(List<Rule> done)
		{
			if (done.Contains(this) || this.Inner == null || this.Ignore || this.Primitive)
				return "<" + (this.Name ?? this.GetType().Name) + ">";
			done.Add(this);
			return (this.Name ?? this.GetType().Name) + (this.Inner != null ? this.Inner.DescribeStructure(done) : "<empty>");
		}

		#region operators
		#region conversions
		public static implicit operator Rule(string str)
		{
			if (str == null || str.Length < 1)
				return new CharTerminal('\0');
			else if (str.Length == 1)
				return new CharTerminal(str[0]);
			else
				return new StringTerminal(str);
		}

		public static implicit operator Rule(char c)
		{
			return new CharTerminal(c);
		}
		public static implicit operator Rule(char[] c)
		{
			return Append(null, c, 0);
		}
		static Rule Append(Rule a, char[] b, int index)
		{
			if (index < b.Length)
			{
				return Append(a | b[index], b, index + 1);
			}
			else
				return a;
		}
		public static implicit operator Rule(System.Text.RegularExpressions.Regex regex)
		{
			return new RegexTerminal(regex);
		}
		#endregion conversions

		#region Or
		public static Rule operator |(Rule a, Rule b)
		{
			if (a != null && b != null)
				return new Or(a, b);

			return a ?? b;
		}
		public static Rule operator |(Rule a, char b)
		{
			if (a != null)
				return new Or(a, b);

			return a ?? b;
		}
		public static Rule operator |(Rule a, char[] b)
		{
			if (a != null && b != null)
				return new Or(a, b);

			return a ?? b;
		}
		public static Rule operator |(Rule a, string b)
		{
			if (a != null)
				return new Or(a, b);

			return a ?? b;
		}
		public static Rule operator |(Rule a, System.Text.RegularExpressions.Regex b)
		{
			if (a != null)
				return new Or(a, b);

			return a ?? b;
		}

		public static Rule operator |(char a, Rule b)
		{
			if (b != null)
				return (Rule)a | b;

			return (Rule)a ?? b;
		}
		public static Rule operator |(string a, Rule b)
		{
			if (a != null && b != null)
				return (Rule)a | b;

			return a ?? b;
		}
		public static Rule operator |(System.Text.RegularExpressions.Regex a, Rule b)
		{
			if (a != null && b != null)
				return (Rule)a | b;

			return a ?? b;
		}

		#endregion Or

		#region And
		public static Rule operator *(Rule a, Rule b)
		{
			if (a != null && b != null)
				return new And(a, b);

			return a ?? b;
		}
		public static Rule operator *(Rule a, char b)
		{
			if (a != null)
				return new And(a, b);

			return a ?? b;
		}
		public static Rule operator *(Rule a, char[] b)
		{
			if (a != null && b != null)
				return new And(a, b);

			return a ?? b;
		}
		public static Rule operator *(Rule a, string b)
		{
			if (a != null)
				return new And(a, b);

			return a ?? b;
		}
		public static Rule operator *(Rule a, System.Text.RegularExpressions.Regex b)
		{
			if (a != null)
				return new And(a, b);

			return a ?? b;
		}

		public static Rule operator *(char a, Rule b)
		{
			if (b != null)
				return (Rule)a * b;

			return (Rule)a ?? b;
		}
		public static Rule operator *(string a, Rule b)
		{
			if (a != null && b != null)
				return (Rule)a * b;

			return a ?? b;
		}
		public static Rule operator *(System.Text.RegularExpressions.Regex a, Rule b)
		{
			if (a != null && b != null)
				return (Rule)a * b;

			return a ?? b;
		}

		#endregion And

		#region Then
		public static Rule operator ^(Rule a, Rule b)
		{
			if (a != null && b != null)
			{
				return new Chain(a, b);
			}
			return a ?? b;
		}

		public static Rule operator ^(Rule a, char b)
		{
			return a ^ (Rule)b;
		}
		public static Rule operator ^(Rule a, string b)
		{
			return a ^ (Rule)b;
		}
		public static Rule operator ^(Rule a, System.Text.RegularExpressions.Regex b)
		{
			return a ^ (Rule)b;
		}

		public static Rule operator ^(char a, Rule b)
		{
			return (Rule)a ^ b;
		}
		public static Rule operator ^(string a, Rule b)
		{
			return (Rule)a ^ b;
		}
		public static Rule operator ^(System.Text.RegularExpressions.Regex a, Rule b)
		{
			return (Rule)a ^ b;
		}
		#endregion Then

		#region Then
		public static Rule operator &(Rule a, Rule b)
		{
			if (a != null && b != null)
			{
				if (b is Optional)
				{
					return a ^ ~(~new Whitespace() ^ b.Inner);
					//return b ^ new Optional(~new Whitespace() ^ b.Inner);
				}
				if (b is Repeat)
				{
					return a ^ b;
					//return b ^ new Repeat(~new Whitespace() ^ b.Inner);
				}
				if (a is Whitespace || b is Whitespace)
					return a ^ b;
				else
					return a ^ ~new Whitespace() ^ b;
			}
			return a ?? b;
		}

		public static Rule operator &(Rule a, char b)
		{
			return a & (Rule)b;
		}
		public static Rule operator &(Rule a, string b)
		{
			return a & (Rule)b;
		}
		public static Rule operator &(Rule a, System.Text.RegularExpressions.Regex b)
		{
			return a & (Rule)b;
		}

		public static Rule operator &(char a, Rule b)
		{
			return (Rule)a & b;
		}
		public static Rule operator &(string a, Rule b)
		{
			return (Rule)a & b;
		}
		public static Rule operator &(System.Text.RegularExpressions.Regex a, Rule b)
		{
			return (Rule)a & b;
		}
		#endregion Then

		#region Optional
		public static Rule operator ~(Rule optional)
		{
			return new Optional(optional);
		}
		#endregion Optional	
		
		#region Exists
		public static Rule operator -(Rule exists)
		{
			return new Exists(exists);
		}
		#endregion Exists

		#region Repeat
		public static Rule operator +(Rule repeat)
		{
			return new Repeat(repeat);
		}
		#endregion

		#region Not
		public static Rule operator !(Rule not)
		{
			return new Not(not);
		}
		#endregion

		#region Name
		public static Rule operator %(string name, Rule rule)
		{
			return new Rule() { Name = name, Inner = rule };
		}
		#endregion

		#endregion operators

		#region IEnumerable<Rule> Members

		public IEnumerator<Rule> GetEnumerator()
		{
			return this.Iterate().GetEnumerator();
		}

		protected virtual IEnumerable<Rule> Iterate()
		{
			yield return this.Inner;
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}
