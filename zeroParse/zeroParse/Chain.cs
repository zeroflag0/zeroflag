using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class Chain : Rule
	{
		protected override Token MatchAll(ParserContext context)
		{
			Token result = this.CreateToken(context, 0);

			Token inner = null;
			//while ((inner = this.MatchWhiteSpace(context.Push(result.Start + result.BlockLength))) != null)
			//    this.AppendToken(result, inner);

			inner = this.MatchInner(context.Push());
			if (inner != null)
			{
				result.Append(inner);
				context.Success = true;

				//while ((inner = this.MatchWhiteSpace(context.Push(result.Start + result.BlockLength))) != null)
				//    this.AppendToken(result, inner);

				inner = this.MatchNext(context.Push(result.Index + result.BlockLength));
				if (inner != null)
				{
					result.Append(inner);
					context.Success = true;
				}
				else
				{
					result = null;
					context.Success = false;
				}
			}
			else
			{
				result = null;
				context.Success = false;
			}


			if (context.Success)
				return result;
			else
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
