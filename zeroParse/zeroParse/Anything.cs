using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class Anything : Rule
	{
		public Anything()
		{
			this.Ignore = true;
		}

		protected override string DefaultName
		{
			get
			{
				return "any";
			}
		}
		protected override Token MatchAll(ParserContext context)
		{
			return this.CreateToken(context, 1);
		}
	}
}
