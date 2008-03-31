using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class EndOfFileException : ParseFailedException
	{
		public EndOfFileException(Rule rule, ParserContext context, string message, Exception inner)
			: base(rule, context, "EOF: " + message, inner)
		{
		}
	}
}
