using System;
using System.Collections.Generic;
using System.Text;

namespace zeroParse
{
	public class Not : Rule
	{
		protected override Token MatchAll(ParserContext context)
		{
			Token result = this.CreateToken(context, 1);

			Token inner = null;
			//while ((inner = this.MatchWhiteSpace(context.Push(result.Start + result.BlockLength))) != null)
			//    this.AppendToken(result, inner);

			context.Success = this.MatchInner(context.Push(result.Start)) == null;
			//context.Success = !context.Success;
			if (!context.Success)
				return null;
			else
				return result;
		}

		public Not()
		{
		}

		public Not(Rule not)
		{
			this.Inner = not;
		}


		public override string ToString()
		{
			return "!( " + this.Inner + " )";
		}
	}
}
