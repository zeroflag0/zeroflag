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
			get { try { return _WhiteSpace ?? (this.WhiteSpace = this.WhiteSpaceCreate); } finally { _WhiteSpace.Name = "space"; _WhiteSpace.Primitive = true; _WhiteSpace.Ignore = true; } }
			set { _WhiteSpace = value; }
		}

		/// <summary>
		/// Creates the default/initial value for WhiteSpaces.
		/// Whitespaces for the parser to ignore/tokenise by.
		/// </summary>
		protected virtual Rule WhiteSpaceCreate
		{
			get { return (CharTerminal)' ' | '\t'; }
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

		public virtual Token Parse(string fileName, string content)
		{
			if (this.Root != null)
			{
				ParserContext context = new ParserContext(this, fileName, "\0" + content + "\0");
				return this.Root.Match(context);
			}

			return null;
		}
	}
}
