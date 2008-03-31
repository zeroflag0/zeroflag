using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class RegexTerminal : Terminal
	{
		private string _Pattern;
		private System.Text.RegularExpressions.Regex _Regex;

		public System.Text.RegularExpressions.Regex Regex
		{
			get { return _Regex ?? (this.Pattern != null ? (_Regex = new System.Text.RegularExpressions.Regex(this.Pattern)) : null); }
			set { _Regex = value; }
		}

		public string Pattern
		{
			get { return _Pattern; }
			set { _Pattern = value; }
		}

		protected override Token MatchThis(ParserContext context)
		{
			if (this.Regex != null)
			{
				var match = this.Regex.Match(context.Source, context.Index);
				if (match != null && match.Success && match.Index == context.Index)
					return this.CreateToken(context, match.Length);
			}
			return null;
		}

		public RegexTerminal()
		{
		}

		public RegexTerminal(string str)
		{
			this.Pattern = str;
		}

		public RegexTerminal(System.Text.RegularExpressions.Regex regex)
		{
			this.Regex = regex;
		}


		public override string ToString()
		{
			return "@\"" + (this.Pattern != null ? System.Text.RegularExpressions.Regex.Escape(this.Pattern) : "<null>") + "\"";
		}

		public override string DescribeStructure(List<Rule> done)
		{
			return "@\"" + (this.Pattern != null ? System.Text.RegularExpressions.Regex.Escape(this.Pattern) : "<null>") + "\"";
		}

	}
}
