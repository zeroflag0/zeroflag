using System;
using System.Collections.Generic;
using System.Text;

namespace zeroParse
{
	public class Debug : Rule
	{
		Rule debug;
		protected override Token MatchAll(ParserContext context)
		{
			return this.debug.Match(context);
		}

		public Debug(Rule debug)
		{
			this.debug = debug;
		}
	}
}
