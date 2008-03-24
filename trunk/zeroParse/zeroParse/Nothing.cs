using System;
using System.Collections.Generic;
using System.Text;

namespace zeroParse
{
	public class Nothing : Rule
	{
		protected override Token MatchAll(ParserContext context)
		{
			return this.CreateToken(context, 0);
		}

		public Nothing()
		{
			this.Ignore = true;
		}

		protected override string DefaultName
		{
			get
			{
				return "nothing";
			}
		}
	}
}
