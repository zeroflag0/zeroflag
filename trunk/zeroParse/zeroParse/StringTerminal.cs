using System;
using System.Collections.Generic;
using System.Text;

namespace zeroParse
{
	public class StringTerminal : Terminal
	{
		private string _String;

		public string String
		{
			get { return _String; }
			set { _String = value; }
		}

		protected override Token MatchThis(ParserContext context)
		{
			if (this.String != null && context.Source.Substring(context.Index, this.String.Length) == this.String)
			{
				return this.CreateToken(context, this.String.Length);
			}
			return null;
		}

		public StringTerminal()
		{
		}

		public StringTerminal(string str)
		{
			this.String = str;
		}


		public override string ToString()
		{
			return "\"" + (this.String != null ? System.Text.RegularExpressions.Regex.Escape(this.String) : "<null>") + "\"";
		}
	}
}
