using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class Chain : Rule
	{
		protected override Token MatchAll(ParserContext context)
		{
			Token first = this.MatchInner(context.Push());
			Token second = null;

			context.Success = false;
			if (first != null)
			{
				second = this.MatchNext(context.Push(first.Index + first.BlockLength));
				if (second != null)
				{
					// chain is only successfull if both parts match...
					context.Success = true;
				}
			}


			if (context.Success)
			{
				if (first != null && second != null)
				{
					Token result = this.CreateToken(context, 0);
					result.Append(first);
					result.Append(second);
					return result;
				}
				else if (first != null)
					return first;
			}
			return null;
		}

		public Chain()
		{
		}

		public Chain(Rule first, Rule next)
		{
			this.Inner = first;
			this.Next = next;
		}

		#region Next

		protected virtual Token MatchNext(ParserContext context)
		{
			return this.Next != null ? this.Next.Match(context) : null;
		}

		private Rule _Next;

		public Rule Next
		{
			get { return _Next; }
			set
			{
				if (_Next != value)
				{
					_Next = value;
				}
			}
		}
		#endregion Next

		public override string ToString()
		{
			return this.Name ?? ("(" + this.Inner + " & " + this.Next + ")");
		}

		public override string DescribeStructure(List<Rule> done)
		{
			if (done.Contains(this) || this.Ignore || this.Primitive)
				return "<" + (this.Name ?? this.GetType().Name) + ">";
			done.Add(this);
			string first = "";
			if (this.Inner != null)
			{
				first = this.Inner.ToString();
				if (this.Inner is Chain && this.Inner.Name == null)
					first = first.Trim().Trim('(', ')').Trim().Trim('&');
			}
			string second = "";
			if (this.Next != null)
			{
				second = this.Next.ToString();
				if (this.Next is Chain && this.Next.Name == null)
					second = second.Trim().Trim('(', ')').Trim().Trim('&');
			}

			if (first.Length / Math.Max(1, first.Split('\n').Length) > ParserContext.SnippetLength)
				first = "\n" + first + "\n";
			if (second.Length / Math.Max(1, second.Split('\n').Length) > ParserContext.SnippetLength)
				second = "\n" + second + "\n";
			return this.Name + "(" +
				first +
				" & " +
				second +
				")";
		}

		protected override IEnumerable<Rule> Iterate()
		{
			yield return this.Inner;
			yield return this.Next;
		}
	}
}
