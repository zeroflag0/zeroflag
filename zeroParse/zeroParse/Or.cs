using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class Or : Rule
	{
		protected override Token MatchAll(ParserContext context)
		{
			Token match = this.Either.Match(context.Push());
			if (match == null || match.Context == null || !match.Context.Success)
				match = this.Otherwise.Match(context.Push());
			if ( match != null )
			{
				context.Success = true;
				return match;//this.CreateToken(context, 0).Append(match);
			}
			else
			{
				context.Errors.Add( new ParseFailedException( this, context, this + " could not match.", null ) );
				return null;
			}
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

		public override string DescribeStructure(List<Rule> done)
		{
			if (done.Contains(this) || this.Ignore || this.Primitive)
				return "<" + (this.Name ?? this.GetType().Name) + ">";
			done.Add(this);
			string first = "";
			if (this.Either != null)
			{
				first = this.Either.ToString();
				if (this.Either is Or && this.Either.Name == null)
					first = first.Trim().Trim('(', ')').Trim().Trim('|');
			}
			string second = "";
			if (this.Otherwise != null)
			{
				second = this.Otherwise.ToString();
				if (this.Otherwise is Or && this.Otherwise.Name == null)
					second = second.Trim().Trim('(', ')').Trim().Trim('|');
			}

			if (first.Length / Math.Max(1, first.Split('\n').Length) > ParserContext.SnippetLength)
				first = "\n" + first + "\n";
			if (second.Length / Math.Max(1, second.Split('\n').Length) > ParserContext.SnippetLength)
				second = "\n" + second + "\n";

			return this.Name + "(" +
				first +
				" | " +
				second +
				")";
		}

		protected override IEnumerable<Rule> Iterate()
		{
			yield return this.Either;
			yield return this.Otherwise;
		}

	}
}
