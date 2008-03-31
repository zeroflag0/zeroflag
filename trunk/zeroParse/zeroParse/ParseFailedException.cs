using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class ParseFailedException : System.Exception
	{
		public ParseFailedException(Rule rule, ParserContext context, string message, Exception inner)
			: base(message ?? (inner != null ? inner.Message : ""), inner)
		{
			this.Rule = rule;
			this.Context = context;
			try { this.Line = context.Source.Substring(0, context.Index).Split('\n').Length; }
			catch { }
		}

		Rule _Rule;

		public Rule Rule
		{
			get { return _Rule; }
			protected set { _Rule = value; }
		}

		ParserContext _Context;

		public ParserContext Context
		{
			get { return _Context; }
			set { _Context = value; }
		}

		List<ParserContext> _ContextTrace = new List<ParserContext>();

		public List<ParserContext> ContextTrace
		{
			get { return _ContextTrace; }
		}

		int _Line;

		public int Line
		{
			get { return _Line; }
			protected set { _Line = value; }
		}


		public override string ToString()
		{
			return this.ToString(true);
		}
		public string ToString(bool showStackTrace)
		{
			if (this.Rule != null && this.Context != null && this.Context.Source != null)
				try
				{
					StringBuilder builder = new StringBuilder();
					builder.Append(this.GetType()).Append(": ").Append(this.Message).AppendLine();
					if (this.Rule != null)
					{
						builder.Append('\t').Append(this.Context.Context).Append(", Line ").Append(this.Line).Append(": ").AppendLine();
						if (this.Context != null)
						{
							builder.Append('\t').Append(this.Context.ToString());
							builder.AppendLine();
						}
					}
					if (showStackTrace)
						builder.Append(this.StackTrace);
					return builder.ToString();
				}
				catch
				{
				}
			return base.ToString();
		}
	}
}
