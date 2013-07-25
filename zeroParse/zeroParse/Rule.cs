using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
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

		public Action OnProcess { get; set; }
		public Action OnMatch { get; set; }

		#region StructureType

		private Type _StructureType;

		/// <summary>
		/// What type of structural item this rule defines.
		/// </summary>
		public Type StructureType
		{
			get { return _StructureType; }
			set
			{
				if (_StructureType != value)
				{
					_StructureType = value;
				}
			}
		}
		#endregion StructureType



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
			if (this.OnProcess != null)
				this.OnProcess();
			context.Rule = this;

			if (context.Depth > MaxDepth)
			{
				int index = context.Index;
				if (context.Result != null)
					index = context.Result.Index + context.Result.Length;
				Console.WriteLine(("Reached " + (index).ToString().PadLeft(6) + " / " + context.Source.Length.ToString().PadRight(6) + ": " + context.ToString().Replace("\0", @"\0").Replace("\n", @"\n").Replace("\t", @"\t")).PadRight(Console.WindowWidth - 2));
				Console.WriteLine();
				Console.WriteLine("Canceling in " + this + " at depth" + context.Depth + ", line" + context.Line + ":\n\t" + context);

				throw new DepthMaxException(this, context, "Canceling in " + this + " at depth" + context.Depth + ", line" + context.Line, null);
			}

			Token result = null;

			result = this.MatchAll(context);

#if !VERBOSE
			if (result != null)
			{
				if (context.Success)
				{
					if (context.Debug && !this.Primitive && !this.Ignore)
					{
						if (this.Name != null)
							Console.WriteLine(new StringBuilder().Append(' ', context.Depth) + this.Name + ": " + context.Line + "." + context.Index + " := " + result.ToString().Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n") + " := " + context.ToString().Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n"));
					}
					//context.Trim();
				}
				if (DateTime.Now - lastOutput > outputInterval)
				{
					Console.Write(("Parsing " + (result.Index + result.Length).ToString().PadLeft(6) + " / " + context.Source.Length.ToString().PadRight(6) + ": " + context.ToString().Replace("\0", @"\0").Replace("\n", @"\n").Replace("\t", @"\t")).PadRight(Console.WindowWidth - 2) + "\r");
					lastOutput = DateTime.Now;
				}
			}
			else
			{
				if (context.Debug && !this.Primitive && !this.Ignore && this.Name != null)
				{
					Console.Write(new StringBuilder().Append(' ', context.Depth) + (this.Name.PadRight(15) + " failed" + context.ToString().Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n")).PadRight(50) + "\r");
				}
				context.Success = false;

				if (context.LastError == null || context.LastError.Rule == null || context.LastError.Rule.Name == null && this.Name != null)
				{
					context.Errors.Add(new ParseFailedException(this, context, this + " could not match.", null));
				}
			}
#endif

			return result;
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
					if (this.OnMatch != null)
						this.OnMatch();
					//result = result ?? this.CreateToken(context, 0);	
					if (result != null)
					{
						result.Append(inner);
					}
					else
					{
						result = inner;
					}
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

			token.Index = context.Index;
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
