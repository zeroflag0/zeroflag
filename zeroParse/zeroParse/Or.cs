using System;
using System.Collections.Generic;
using System.Text;

namespace zeroParse
{
	public class Or : Rule
	{
		protected override Token MatchAll(ParserContext context)
		{
			Token match = this.Either.Match(context.Push()) ?? this.Otherwise.Match(context.Push());
			if (match != null)
				return this.CreateToken(context, 0).Append(match);
			else
				return null;
		}

		public Or()
		{
		}

		public Or(Rule either, Rule or)
		{
			this.Either = either;
			this.Otherwise = or;
		}

		#region Either

		private Rule _Either;

		public Rule Either
		{
			get { return _Either; }
			set
			{
				if (_Either != value)
				{
					_Either = value;
				}
			}
		}
		#endregion Either

		#region Or

		private Rule _Otherwise;

		public Rule Otherwise
		{
			get { return _Otherwise; }
			set
			{
				if (_Otherwise != value)
				{
					_Otherwise = value;
				}
			}
		}
		#endregion Or

		public override string ToString()
		{
			return this.Name ?? ("(" + this.Either + " | " + this.Otherwise + ")");
		}

		protected override IEnumerable<Rule> Iterate()
		{
			yield return this.Either;
			yield return this.Otherwise;
		}

	}
}
