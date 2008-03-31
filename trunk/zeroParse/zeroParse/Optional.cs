using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class Optional : Rule
	{
		protected override Token MatchAll(ParserContext context)
		{
			Token result = this.MatchInner(context);
			if (result == null || result.BlockLength == 0 || result.BlockValue == "\0")
				return this.CreateToken(context, 0);
			else
				return result;
		}
		public Optional()
		{
		}

		public Optional(Rule optional)
		{
			this.Inner = optional;
		}

		public override string ToString()
		{
			return this.Name ?? "[ " + this.Inner + " ]";
		}

		public override string DescribeStructure(List<Rule> done)
		{
			if (done.Contains(this) || this.Inner == null || this.Ignore || this.Primitive)
				return "<" + (this.Name ?? this.GetType().Name) + ">";
			done.Add(this);
			return (this.Name ?? "~") + (this.Inner != null ? this.Inner.DescribeStructure(done) : "<empty>");
		}
	}
}
