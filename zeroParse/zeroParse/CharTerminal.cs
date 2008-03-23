using System;
using System.Collections.Generic;
using System.Text;

namespace zeroParse
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
			if (context.Source[context.Index] == this.Char)
				return this.CreateToken(context, 1);
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
	}
}
