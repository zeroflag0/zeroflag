using System;
using System.Collections.Generic;
using System.Text;

namespace zeroParse
{
	public class Token
	{
		private int _Start;

		public int Start
		{
			get { return _Start; }
			set { _Start = value; }
		}
		private int _Length;

		public int Length
		{
			get { return _Length; }
			set { _Length = value; }
		}
		private string _Value;

		public string Value
		{
			get { return _Value; }
			set { _Value = value; }
		}
		private string _Name;

		public string Name
		{
			get { return _Name ?? (this.Rule != null ? this.Rule.Name : null) ?? (this.Context != null ? (this.Context.Rule != null ? this.Context.Rule.Name : null) : null); }
			set { _Name = value; }
		}
		private Token _Outer;

		public Token Outer
		{
			get { return _Outer; }
			set { _Outer = value; }
		}
		private List<Token> _Inner = new List<Token>();

		public List<Token> Inner
		{
			get { return _Inner; }
		}
		public Token Append(Token other)
		{
			if (other != null)
				this.Inner.Add(other);
			return this;
		}

		public int BlockLength
		{
			get
			{
				int length = this.Length;
				foreach (Token inner in this.Inner)
				{
					length += inner.BlockLength;
				}
				return length;
			}
		}

		public string BlockValue
		{
			get
			{
				try
				{
					return this.Context.Source.Substring(this.Start, this.BlockLength);
				}
				catch
				{
					return this.Value;
				}
			}
		}

		private Rule _Rule;

		public Rule Rule
		{
			get { return _Rule; }
			set { _Rule = value; }
		}

		#region Context

		private ParserContext _Context = default(ParserContext);

		public ParserContext Context
		{
			get { return _Context; }
			set
			{
				if (_Context != value)
				{
					_Context = value;
				}
			}
		}
		#endregion Context

		public override string ToString()
		{
			return this.BlockValue ?? this.Name;
		}

		public string ToStringTree()
		{
			return this.ToString(new StringBuilder(), 0).ToString();
		}

		public StringBuilder ToString(StringBuilder builder, int depth)
		{
			string value = this.BlockValue;
			if ((this.Name != null && this.Name.Length > 0 && value != null && value.Length > 0) && (this.Rule == null || !this.Rule.Ignore))
			{
				//builder.Append('+', depth);
				if (this.Name != value && this.Name != null && this.Name.Length > 0)
					builder.Append(this.Name).Append("=");
				if (value != null && value.Length > 0)
				{
					value = value.Replace("\t", @"\t  ");
					if (value.Length > ParserContext.SnippetLength)
					{
						builder.Append("'").AppendLine().Append(value.Replace("\r\n", "\n").Replace("\n", "\\n\n")).AppendLine().Append("'");
					}
					else
					{
						builder.Append("'").Append(value.Replace("\r\n", "\\n").Replace("\n", "\\n")).Append("'");
					}
				}
				builder.AppendLine();
			}
			foreach (Token inner in this.Inner)
				inner.ToString(builder, depth + 1);
			return builder;
		}
	}
}
