using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class Whitespace : Repeat
	{
		public Whitespace()
		{
			this.Ignore = true;
			this.Primitive = true;
		}

		public Whitespace(Rule inner)
			: this()
		{
			this.Inner = inner;
		}

		protected override string DefaultName
		{
			get
			{
				return "space";
			}
		}

		protected override Token MatchInner(ParserContext context)
		{
			if (context.WhiteSpaces is Whitespace)
				return context.WhiteSpaces.Inner.Match(context);
			else
				return context.WhiteSpaces.Match(context);
		}

		public override string ToString()
		{
			return "' '";
		}

		public override string DescribeStructure(List<Rule> done)
		{
			return "' '";
		}
	}
}
