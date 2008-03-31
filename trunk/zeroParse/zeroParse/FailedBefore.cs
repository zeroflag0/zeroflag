using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class FailedBefore : Rule
	{

		public FailedBefore(Rule inner)
		{
			this.Inner = inner;
		}

		protected override Token MatchAll(ParserContext context)
		{
			Token result = null;
			ParseFailedException previous = context.LastError;
			try
			{
				result = this.Inner.Match(context);
			}
			catch (ParseFailedException)
			{
				//context.Errors.Add(exc);
				throw context.LastError;
			}
			finally
			{
			}
			context.Errors.Add(previous);
			return result;
		}
	}
}
