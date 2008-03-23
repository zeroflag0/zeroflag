using System;
using System.Collections.Generic;
using System.Text;

namespace zeroParse
{
	public class Repeat : Rule
	{
		protected override Token MatchAll(ParserContext context)
		{
			Token result = this.CreateToken(context, 0);

			Token inner = null;
			//while ((inner = this.MatchWhiteSpace(context.Push(result.Start + result.BlockLength))) != null)
			//    this.AppendToken(result, inner);

			context.Success = false;
			while ((inner = this.MatchInner(context.Push(result.Start + result.BlockLength))) != null)
			{
				if (inner != null)
				{
					result.Append(inner);
					context.Success = true;
				}
			}
			if (context.Success)
				return result;
			else
				return null;
		}
		public Repeat()
		{
		}

		public Repeat(Rule repeat)
		{
			this.Inner = repeat;
		}

		public override string ToString()
		{
			return "{ " + this.Inner + " }";
		}
	}
}
