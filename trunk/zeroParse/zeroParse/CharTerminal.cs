using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class CharTerminal : Terminal
	{
		private char _Char;

		public CharTerminal()
		{
		}

		public CharTerminal(char c)
		{
			this.Char = c;
		}

		public char Char
		{
			get { return _Char; }
			set { _Char = value; }
		}

		protected override Token MatchThis(ParserContext context)
		{
			if (context.Source.Length > context.Index && context.Source[context.Index] == this.Char)
				return this.CreateToken(context, 1);
			context.Errors.Add(new ParseFailedException(this, context, this + " cannot match: \'" + this.Char.ToString() + "\'", null));
			return null;
		}

		protected override void FillToken(Token token)
		{
			base.FillToken(token);
			token.Name = this.Char.ToString();
		}

		public override string ToString()
		{
			return "'" + System.Text.RegularExpressions.Regex.Escape(this.Char.ToString()) + "'";
		}

		public override string DescribeStructure(List<Rule> done)
		{
			return "'" + this.Char.ToString().Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n") + "'";
		}
	}
}
