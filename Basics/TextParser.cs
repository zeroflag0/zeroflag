using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basics.Parsing
{
	public abstract class BaseMatch
	{
		public BaseMatch()
		{
			this.Maximum = 1;
			this.Minimum = 1;
		}

		public int Maximum { get; set; }
		public int Minimum { get; set; }

		public virtual string Description
		{
			get
			{
				string n;
				if (this.Minimum == this.Maximum)
					if (this.Minimum == 1)
						n = "";
					else
						n = "{" + this.Minimum + "}";
				else
					n = "{" + this.Minimum + "-" + this.Maximum + "}";
				return n;
			}
		}
		public override string ToString()
		{
			return this.Description + (this.Next == null ? "" : " -> " + this.Next);
		}

		public ResultMatch Set(Action<string> callback, string name = null)
		{
			return new ResultMatch(this, callback, name);
		}

		public bool IsMatch(string line)
		{
			return this.Match(line) != null;
		}

		public string Match(string text)
		{
			Parser parser = new Parser(text);
			return this.Match(parser);
		}

		public string Match(Parser parser)
		{
			string result = this.MatchSelf(parser);
			if (result != null && this.Next != null)
			{
				string sub = this.Next.Match(parser);
				if (sub == null)
					return null;
				result += sub;
			}
			return result;
		}

		protected abstract string MatchSelf(Parser parser);

		public BaseMatch Next { get; private set; }
		public BaseMatch Queue(BaseMatch next)
		{
			if (this.Next == null)
				this.Next = next;
			else
				this.Next.Queue(next);
			return this;
		}

		public static BaseMatch operator +(BaseMatch a, BaseMatch b)
		{
			a = a.CloneBase();
			a.Queue(b);
			return a;
		}

		public static ResultMatch operator ~(BaseMatch match)
		{
			return new ResultMatch(match);
		}

		public abstract BaseMatch CloneBase();
	}

	public abstract class BaseMatch<TSelf> : BaseMatch
		where TSelf : BaseMatch<TSelf>, new()
	{
		public TSelf this[int repetitions]
		{
			get
			{
				return this[repetitions, repetitions];
			}
		}

		public TSelf this[int min, int max]
		{
			get
			{
				TSelf clone = this.Clone();
				clone.Minimum = min;
				clone.Maximum = max;
				return clone;
			}
		}

		public override BaseMatch CloneBase()
		{
			return this.Clone();
		}
		public TSelf Clone()
		{
			TSelf clone = new TSelf();
			clone.Minimum = this.Minimum;
			clone.Maximum = this.Maximum;
			if (this.Next != null)
				clone.Queue(this.Next.CloneBase());
			this.Clone(clone);
			return clone;
		}

		protected abstract void Clone(TSelf clone);
	}

	public class ResultMatch : BaseMatch<ResultMatch>
	{
		public ResultMatch()
		{
		}
		public ResultMatch(BaseMatch match)
		{
			this.Result = match;
		}
		public ResultMatch(BaseMatch match, Action<string> callback)
			: this(match)
		{
			this.Callback = callback;
		}
		public ResultMatch(BaseMatch match, Action<string> callback, string name)
			: this(match, callback)
		{
			this.Name = name;
		}

		public override string Description
		{
			get
			{
				return "<" + (this.Name ?? "" + Callback) + ">" + this.Result;
			}
		}

		public string Name { get; set; }
		public BaseMatch Result { get; set; }
		public Action<string> Callback { get; set; }

		public ResultMatch Set(Action<string> callback)
		{
			this.Callback = callback;
			return this;
		}

		protected override void Clone(ResultMatch clone)
		{
			clone.Name = this.Name;
			clone.Callback = this.Callback;
			clone.Result = this.Result.CloneBase();
		}

		protected override string MatchSelf(Parser parser)
		{
			string result = this.Result.Match(parser);
			if (this.Callback != null)
				this.Callback(result);
			return result;
		}
	}

	public class AnyCharMatch : BaseMatch<AnyCharMatch>
	{
		public override string Description
		{
			get { return "*" + base.Description; }
		}

		protected override string MatchSelf(Parser parser)
		{
			int max = this.Maximum;
			if (parser.Index + max >= parser.Text.Length)
				max = parser.Text.Length - parser.Index;

			string c = parser.Text.Substring(parser.Index, max);
			parser.Increment(max);
			return c;
		}

		protected override void Clone(AnyCharMatch clone)
		{
		}
	}

	public class CharMatch : BaseMatch<CharMatch>
	{
		public CharMatch() : base() { }

		private HashSet<char> _Matches;
		/// <summary>
		/// matches
		/// </summary>
		public HashSet<char> Matches
		{
			get { return _Matches; }
			private set
			{
				if (_Matches != value)
				{
					_Matches = value;
					_AllChar = null;
				}
			}
		}

		protected override void Clone(CharMatch clone)
		{
			clone._AllChar = this._AllChar;
			clone.Matches = new HashSet<char>(this.Matches);
		}
		string _AllChar;
		public string AllChar
		{
			get
			{
				var matches = this.Matches;
				if (_AllChar == null && matches != null)
					lock (matches)
						if (_AllChar == null && matches != null)
						{
							StringBuilder chars = new StringBuilder();
							foreach (char c in matches)
							{
								chars.Append(c);
							}
							_AllChar = chars.ToString();
						}
				return _AllChar;
			}
		}
		public override string Description
		{
			get { return "[" + this.AllChar + "]" + base.Description; }
		}

		protected override string MatchSelf(Parser parser)
		{
			int max = this.Maximum;
			if (max == 0)
				return null;
			if (max == 1)
			{
				if (this.Matches.Contains(parser.CurrentChar))
				{
					string c = parser.CurrentChar.ToString();
					parser.Increment();
					return c;
				}
				else
					return null;
			}
			if (max < 0)
				max = int.MaxValue;

			StringBuilder match = new StringBuilder();
			for (int i = 0; i < max && this.Matches.Contains(parser.CurrentChar); i++)
			{
				match.Append(parser.CurrentChar);
				parser.Increment();
			}
			if (this.Minimum < 0)
			{
				if (match.Length < 1)
					return null;
			}
			else
				if (match.Length < this.Minimum)
					return null;
			return match.ToString();
		}

		public CharMatch(string characters)
			: this(characters.ToCharArray())
		{
			this._AllChar = characters;
		}

		public CharMatch(IEnumerable<char> characters)
		{
			Matches = new HashSet<char>(characters);
		}

		public static implicit operator CharMatch(char[] a)
		{
			return new CharMatch(a);
		}

		public static CharMatch operator |(CharMatch a, CharMatch b)
		{
			CharMatch m = a.Clone();
			foreach (char c in b.Matches)
			{
				if (!m.Matches.Contains(c))
					m.Matches.Add(c);
			}
			return m;
		}

		public static CharMatch operator |(CharMatch a, IEnumerable<char> b)
		{
			CharMatch m = a.Clone();
			foreach (char c in b)
			{
				if (!m.Matches.Contains(c))
					m.Matches.Add(c);
			}
			return m;
		}

		public static CharMatch operator |(CharMatch a, char c)
		{
			CharMatch m = a.Clone();
			m.Matches.Add(c);
			return m;
		}
		public static CharMatch operator |(CharMatch a, string b)
		{
			return new CharMatch(b) | a;
		}
	}

	public class Parser
	{
		public Parser()
		{
		}
		public Parser(string text)
		{
			this.Text = text;
			this._Index = 0;
		}

		public string Match(string text)
		{
			this.Text = text;
			this._Index = 0;
			return this.Definition.Match(this);
		}

		public override string ToString()
		{
			return "{" + this.Text + "}[" + this.Index + ":" + this.CurrentChar + "]";
		}
		public char CurrentChar
		{
			get
			{
				if (this.Index >= this.Text.Length)
					return '\0';
				return this.Text[this.Index];
			}
		}
		public string Text { get; set; }
		int _Index;
		public int Index { get { return _Index; } }
		public int Increment(int count = 1)
		{
			if (count == 1)
				System.Threading.Interlocked.Increment(ref _Index);
			else
				lock (this)
					for (int i = 0; i < count; i++)
						System.Threading.Interlocked.Increment(ref _Index);
			return _Index;
		}

		public BaseMatch Definition { get; set; }

		/// <summary>
		/// abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789
		/// </summary>
		public static CharMatch WordCVar = new CharMatch("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789");
		/// <summary>
		/// abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ
		/// </summary>
		public static CharMatch Word = new CharMatch("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
		/// <summary>
		/// 0123456789
		/// </summary>
		public static CharMatch Decimal = new CharMatch("0123456789");
		public static CharMatch NonSpace = Word | Decimal | "_";
		/// <summary>
		/// 0123456789abcdefABCDEF
		/// </summary>
		public static CharMatch Hexadecimal = new CharMatch("0123456789abcdefABCDEF");
		/// <summary>
		/// 0123456789.
		/// </summary>
		public static CharMatch Number = new CharMatch("0123456789.");
		/// <summary>
		///  \t
		/// </summary>
		public static CharMatch Space = new CharMatch(" \t");
	}
}
