using System;
using System.Collections.Generic;
using System.Text;

namespace zeroParse
{
	public class Parser
	{
		private Rule _Root;

		public Rule Root
		{
			get { return _Root ?? (_Root = this.CreateRules()); }
			set { _Root = value; }
		}

		#region WhiteSpaces
		private Rule _WhiteSpace;

		/// <summary>
		/// Whitespaces for the parser to ignore/tokenise by.
		/// </summary>
		public Rule WhiteSpace
		{
			get { try { return (_WhiteSpace as Whitespace) ?? (this.WhiteSpace = new Whitespace(_WhiteSpace ?? this.WhiteSpaceCreate)); } finally { _WhiteSpace.Name = "space"; _WhiteSpace.Primitive = true; _WhiteSpace.Ignore = true; } }
			set { _WhiteSpace = value; }
		}

		/// <summary>
		/// Creates the default/initial value for WhiteSpaces.
		/// Whitespaces for the parser to ignore/tokenise by.
		/// </summary>
		protected virtual Rule WhiteSpaceCreate
		{
			get { return (CharTerminal)' ' | '\t' | '\0'; }
		}

		#endregion WhiteSpaces

		#region Letter
		private Rule _Letter;

		/// <summary>
		/// Letters for the parser to recognize.
		/// </summary>
		public Rule Letter
		{
			get { try { return _Letter ?? (_Letter = this.LetterCreate); } finally { _Letter.Name = "letter"; _Letter.Primitive = true; } }
		}

		/// <summary>
		/// Creates the default/initial value for Letter.
		/// Letters for the parser to recognize.
		/// </summary>
		protected virtual Rule LetterCreate
		{
			get { return (Rule)"abcdefghijklmnopqrstuvwxyz".ToCharArray() | "abcdefghijklmnopqrstuvwxyz".ToUpper().ToCharArray(); }
		}

		#endregion Letter

		protected virtual string Preprocess(string content)
		{
			content = content.Replace("\r\n", "\n");
			return content;
		}

		public virtual Rule CreateRules()
		{
			return null;
		}

		public Token Parse(string fileName)
		{
			return this.Parse(fileName, new System.IO.StreamReader(fileName));
		}

		public Token Parse(string fileName, System.IO.TextReader reader)
		{
			return this.Parse(fileName, reader.ReadToEnd());
		}

		public Token Parse(string fileName, out ParserContext context)
		{
			return this.Parse(fileName, new System.IO.StreamReader(fileName), out context);
		}

		public Token Parse(string fileName, System.IO.TextReader reader, out ParserContext context)
		{
			return this.Parse(fileName, reader.ReadToEnd(), out context);
		}

		public Token Parse(string fileName, string content)
		{
			ParserContext context;
			return this.Parse(fileName, content, out context);
		}

		public virtual Token Parse(string fileName, string content, out ParserContext context)
		{
			if (this.Root != null)
			{
				Console.Write("Preprocessing... ");
				content = this.Preprocess(content);
				//System.IO.File.WriteAllText(fileName + ".pp.cpp", content);
				Console.WriteLine("done.");
				context = new ParserContext(this, fileName, content);
				return this.Root.Match(context);
			}
			else
				context = null;
			return null;
		}
	}
}
