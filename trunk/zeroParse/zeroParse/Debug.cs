using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class Debug : Rule
	{
		Rule debug;
		protected override Token MatchAll(ParserContext context)
		{
			Console.WriteLine("<debug rule='" + (this.debug != null ? (this.debug.Name ?? this.debug.Structure.Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n")) : "~~null~~") + "' \n\tcontext='" + context.ToString().Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n") + "'>");
			context.Debug = true;
			try
			{
				return this.debug.Match(context);
			}
			finally
			{
				Console.WriteLine();
				Console.WriteLine("</debug>\n<result success='" + context.Success + "' rule='" + (this.debug != null ? (this.debug.Name ?? this.debug.Structure.Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n")) : "~~null~~") + "' \n\tcontext='" + context.ToString().Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n") + "'>");
				Console.WriteLine(context.Result);
				Console.WriteLine("</result>");
			}
		}

		public Debug(Rule debug)
		{
			this.debug = debug;
		}
	}
}
