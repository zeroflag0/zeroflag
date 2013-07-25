using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace zeroflag.Parsing
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
			while ((inner = this.MatchInner(context.Push(result.Index + result.BlockLength))) != null)
			{
				if (inner != null)
				{
					result.Append(inner);
					context.Success = true;
				}
			}
			if (context.Success)
			{
				if (result.Inner.Count == 1)
					return result.Inner.First();
				else
					return result;
			}
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

		public override string DescribeStructure(List<Rule> done)
		{
			if (done.Contains(this) || this.Inner == null || this.Ignore || this.Primitive)
				return "<" + (this.Name ?? this.GetType().Name) + ">";
			done.Add(this);
			return (this.Name ?? "+") + (this.Inner != null ? this.Inner.DescribeStructure(done) : "<empty>");
		}
	}
}
