using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace zeroflag.BridgeGenerator
{
	public class Metadata
	{
		Dictionary<string, List<Member>> _Members = new Dictionary<string, List<Member>>();

		public Dictionary<string, List<Member>> Members
		{
			get { return _Members; }
		}

		public void SetMembers(IEnumerable<Member> members)
		{
			this.Members.Clear();
			foreach (Member member in members)
			{
				if (!this.Members.ContainsKey(member.Name))
					this.Members.Add(member.Name, new List<Member>());

				this.Members[member.Name].Add(member);
			}
		}

		List<string> Usings = new List<string>();

		#region Scopes
		ScopeResult _Scopes;

		public ScopeResult Scopes
		{
			get { return _Scopes; }
			set { _Scopes = value; }
		}
		public class ScopeResult
		{
			private string _Value;

			public string Value
			{
				get { return _Value; }
				set { _Value = value; }
			}
			private int _Index;

			public int Index
			{
				get { return _Index; }
				set { _Index = value; }
			}
			private int _Length;

			public int Length
			{
				get { return _Length; }
				set { _Length = value; }
			}
			List<ScopeResult> _Inner = new List<ScopeResult>();

			public List<ScopeResult> Inner
			{
				get { return _Inner; }
				set { _Inner = value; }
			}

			ScopeResult Add(ScopeResult inner)
			{
				if (inner != null)
					this.Inner.Add(inner);
				return inner;
			}

			public ScopeResult() { }
			public ScopeResult(Match match) { this.Value = match.Result("${scope}"); this.Index = match.Index; this.Length = match.Length; }
			public ScopeResult(string value, int start) { this.Value = value; this.Index = start; this.Length = this.Value.Length; }

			public override string ToString() { return this.Value; }

			public ScopeResult Scan()
			{
				return this.Scan(this.Value, 0);
			}

			public ScopeResult Scan(string input, int current)
			{
				return this.Scan(input, ref current);
			}
			public ScopeResult Scan(string input, ref int current)
			{
				int start = input.IndexOf("{", current);
				if (start < 0)
					return null;

				this.Index = start + 1;

				int end = input.IndexOf("}", this.Index);
				int next = input.IndexOf("{", this.Index);
				if (next > this.Index && next < end)
				{
					ScopeResult inner;
					do
					{
						inner = new ScopeResult();
					}
					while (this.Add(inner.Scan(input, ref next)) != null);
					if (next >= 0 && next < input.Length)
						end = input.IndexOf("}", next);
					else
						end = -1;
				}
				if (end < 0)
					return null;
				current = end;
				this.Length = end - this.Index;
				this.Value = input.Substring(this.Index, this.Length);
				return this;

				//int start = input.IndexOf("{", current);
				//Console.WriteLine("Start at " + start);
				//if (start < 0)
				//    return null;
				//else
				//{
				//    current = start;
				//    int next = this.Index = start + 1;
				//    int end;
				//    for (; ; )
				//    {
				//        end = input.IndexOf("}", next);
				//        next = input.IndexOf("{", next);
				//        if (end < 0)
				//        {
				//            Console.WriteLine("No end found...");
				//            return null;
				//        }

				//        if (next < 0)
				//        {
				//            // there are no more inner blocks...
				//            Console.WriteLine("No more inner blocks, end at " + end);
				//            break;
				//        }

				//        if (end < next)
				//        {
				//            // an end was found...
				//            Console.WriteLine("End before next, end at " + end);
				//            break;
				//        }

				//        ScopeResult inner = new ScopeResult();
				//        inner.Index = next;
				//        this.Add(inner.Scan(input, ref next));
				//    }
				//    current = end;
				//    this.Length = end - this.Index;
				//    this.Value = input.Substring(this.Index, this.Length);
				//    return this;
				//}
			}
		}
		#endregion

		//        Dictionary<string,string> Scopes;

		//public Dictionary<string, string> Scopes
		//{
		//  get { return Scopes ?? (Scopes = this.CreateScopes); }
		//}
		//        protected Dictionary<string, string> CreateScopes
		//        {
		//            get
		//            {
		//                Dictionary<string, string> scopes = new Dictionary<string,string>();
		//                scopes.Add("{","}");
		//                scopes.Add("(", ")");
		//                scopes.Add("[", "]");
		//                scopes.Add("\"", "\"");
		//                return scopes;
		//            }
		//        }

		static Regex Comment = new Regex(@"\/\/(?<comment>.*)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
		static Regex Attribute = new Regex(
			@"\[(?<attribute>
				(?>
						[^\[\]]*	|
					\[	(?<attCount>)	|
					\]	(?<-attCount>)
				)*
				(?(attCount)(?!)))
				\]"
			, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace);
		static Regex Scope = new Regex(
			@"\{(?<scope>
				(?>
						[^\{\}]*	|
					\{	(?<scopeCount>)	|
					\}	(?<-scopeCount>)
				)*
				(?(scopeCount)(?!)))
				\}"
			, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
		static Regex Namespace = new Regex(
			@"\s*(namespace)\s+(?<namespace>[\w\d\.\:]+)\s*
				\{(?<scope>
				(?>
						[^\{\}]*	|
					\{	(?<scopeCount>)	|
					\}	(?<-scopeCount>)
				)*
				(?(scopeCount)(?!)))
				\}"
			, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
		static Regex Using = new Regex(@"using\s+(?<using>[^\;\n]+)\;$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

		public void Run(string input)
		{
			Console.WriteLine();
			Console.WriteLine();

			input.Replace("\r\n", "\r");

			MatchCollection usings = Using.Matches(input);
			this.Usings.Clear();
			foreach (Match use in usings)
				this.Usings.Add(use.Result("${using}"));

			this.Scopes = new ScopeResult(input, 0);
			this.Scopes.Scan();

			//for (int i = 0; i < scopes.Count; i++)
			//    Console.WriteLine("<scope" + i + ">\n" + scopes[i] + "\n<scope" + i + ">\n");


		}




	}
}
