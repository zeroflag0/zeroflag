using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class DepthMaxException : ParseFailedException
	{
		public DepthMaxException(Rule rule, ParserContext context, string message, Exception inner)
			: base(rule, context, "DepthMax: " + message, inner)
		{
		}
	}
}
