using System;
using System.Collections.Generic;
using System.Text;

namespace zeroParse
{
	public class Exists : Rule
	{
		public override string DescribeStructure(List<Rule> done)
		{
			if (done.Contains(this) || this.Inner == null || this.Ignore || this.Primitive)
				return "<" + (this.Name ?? this.GetType().Name) + ">";
			done.Add(this);
			return (this.Name ?? "-") + (this.Inner != null ? this.Inner.DescribeStructure(done) : "<empty>");
		}

		protected override Token MatchAll(ParserContext context)
		{
			Token result = this.MatchInner(context);
			if (result == null || result.BlockLength == 0 || result.BlockValue == "\0")
				return null;
			else
				return this.CreateToken(context, 0);
		}

		public override string ToString()
		{
			return this.Name ?? "-[ " + this.Inner + " ]";
		}

		public Exists()
		{
		}

		public Exists(Rule exists)
		{
			this.Inner = exists;
		}
	}
}
