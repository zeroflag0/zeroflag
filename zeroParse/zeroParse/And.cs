using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class And : Rule
	{
		private Rule _First;
		public Rule First
		{
			get { return _First; }
			set
			{
				if (_First != value)
				{
					_First = value;
				}
			}
		}


		private Rule _Second;
		public Rule Second
		{
			get { return _Second; }
			set
			{
				if (_Second != value)
				{
					_Second = value;
				}
			}
		}

		public override string DescribeStructure(List<Rule> done)
		{
			if (done.Contains(this) || this.Ignore || this.Primitive)
				return "<" + (this.Name ?? this.GetType().Name) + ">";
			done.Add(this);
			string first = "";
			if (this.First != null)
			{
				first = this.First.ToString();
				if (this.First is And && this.First.Name == null)
					first = first.Trim().Trim('(', ')').Trim().Trim('|');
			}
			string second = "";
			if (this.Second != null)
			{
				second = this.Second.ToString();
				if (this.Second is And && this.Second.Name == null)
					second = second.Trim().Trim('(', ')').Trim().Trim('|');
			}

			if (first.Length / Math.Max(1, first.Split('\n').Length) > ParserContext.SnippetLength)
				first = "\n" + first + "\n";
			if (second.Length / Math.Max(1, second.Split('\n').Length) > ParserContext.SnippetLength)
				second = "\n" + second + "\n";

			return this.Name + "(" +
				first +
				" && " +
				second +
				")";
		}

		protected override IEnumerable<Rule> Iterate()
		{
			yield return this.First;
			yield return this.Second;
		}

		protected override Token MatchAll(ParserContext context)
		{
			Token first = this.First.Match(context.Push());
			Token second = null;
			if (first == null || first.Context == null || !first.Context.Success)
				context.Success = false;
			else
				second = this.Second.Match(context.Push());

			if (first != null && first.Context != null && first.Context.Success&&
				second != null && second.Context != null && second.Context.Success)
				return this.CreateToken(context, 0).Append(first);
			else
				return null;
		}

		public override string ToString()
		{
			return this.Name ?? ("(" + this.First + " | " + this.Second + ")");
		}

		public And()
		{
		}

		public And(Rule either, Rule or)
		{
			this.First = either;
			this.Second = or;
		}
	}
}
