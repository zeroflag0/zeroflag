﻿using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public class ParserContext
	{
		#region Properties
		private string _Source;
		private int _Index;
		private string _Context;
		private Rule _Rule;
		private Parser _Parser;
		private List<ParserContext> _Inner;
		private ParserContext _Outer;
		private bool _Success = false;
		Token _Result;
		bool _Debug = false;

		public bool Debug
		{
			get { return _Debug || ( this.Outer != null && this.Outer.Debug ); }
			set { _Debug = value; }
		}

		public Token Result
		{
			get { return _Result; }
			set { _Result = value; }
		}

		public ParserContext Outer
		{
			get { return _Outer; }
			set { if ( value == this || ( value != null && value.Outer == this ) ) throw new ArgumentException( "reference loop" ); _Outer = value; }
		}

		public bool Success
		{
			get { return _Success; }
			set { _Success = value; }
		}

		public List<ParserContext> Inner
		{
			get { return _Inner ?? ( _Inner = new List<ParserContext>() ); }
		}

		public Parser Parser
		{
			get { return _Parser ?? ( this.Outer != null ? this.Outer.Parser : null ); }
			set { _Parser = value; }
		}

		[System.ComponentModel.ReadOnly( true )]
		public Rule Rule
		{
			get { return _Rule ?? ( this.Result != null ? this.Result.Rule : null ); }
			set { _Rule = value; }
		}

		public string Context
		{
			get { return _Context ?? ( this.Outer != null ? this.Outer.Context : null ); }
			set { _Context = value; }
		}

		public int Index
		{
			get { return _Index; }
			set { _Index = value; }
		}

		public string Source
		{
			get { /*Console.Write(this.Depth.ToString().PadLeft(3) + "\r"); */return _Source ?? ( this.Outer != null ? this.Outer.Source : null ); }
			set { _Source = value; }
		}

		public int Depth
		{
			get { return this.Outer != null ? this.Outer.Depth + 1 : 0; }
		}

		public int InnerDepth
		{
			get
			{
				int max = 0;
				int temp = 0;
				try
				{
					foreach ( ParserContext inner in this.Inner )
					{
						temp = inner.InnerDepth;
						if ( temp > max )
							max = temp;
					}
				}
				catch { }
				return max;
			}
		}

		public int Line
		{
			get { return this.Source.Substring( 0, this.Index ).Split( '\n' ).Length; }
		}

		public Rule WhiteSpaces
		{
			get { return this.Parser.WhiteSpace; }
		}

		public Rule Root
		{
			get { return this.Parser.Root; }
		}


		#region Errors

		private List<ParseFailedException> _Errors;

		public List<ParseFailedException> Errors
		{
			get { return this.Outer != null ? this.Outer.Errors : ( this._Errors ?? ( this._Errors = new List<ParseFailedException>() ) ); }
		}

		public ParseFailedException LastError
		{
			get { try { return this.Errors[ this.Errors.Count - 1 ]; } catch { return null; } }
		}
		#endregion Errors

		#endregion Properties

		#region Constructors
		public ParserContext()
		{
		}

		public ParserContext( Parser parser, string context, string source )
		{
			this.Parser = parser;
			this.Context = context;
			this.Source = source;
		}

		public ParserContext( Parser parser, string context, string source, int index )
			: this( parser, context, source )
		{
			this.Index = index;
		}

		public ParserContext( ParserContext outer, int index )
		{
			this.Outer = outer;
			this.Index = index;
		}
		#endregion Constructors

		#region Methods
		public ParserContext Push()
		{
			return this.Push( this.Index );
		}
		public ParserContext Push( int index )
		{
			var cont = new ParserContext( this, index );
			this.Inner.Add( cont );
			return cont;
		}

		#region Trim
		public ParserContext Trim()
		{
#if TRIM
			List<ParserContext> inners = new List<ParserContext>( this.Inner );
			foreach ( ParserContext inner in inners )
			{
				if ( inner.AllowTrim )
				{
					Console.Write( ( "Removed(trimmed) context " + inner.Rule + " " + inner.ToString().Replace( "\r\n", "" ).Replace( "\n", "" ) + "..." ).PadRight( Console.WindowWidth - 2 ) + "\r" );
					this.Inner.Remove( inner );
				}
				else
				{
					inner.Trim();
					if ( inner.Result != null )
						inner.Result.Trim();
				}
			}
#endif
			this.OnTrimmed( this );
			return this;
		}

		public bool AllowTrim
		{
			get
			{
				//if ( this.Rule is Or )
				//    return false;
				if ( !this.Success )
					return true;
				if ( this.Inner.Count <= 0 && this.Result == null || this.Rule.Primitive || this.Rule.Ignore || ( this.Rule.Inner != null && ( this.Rule.Inner.Ignore || this.Rule.Inner.Primitive ) ) )
					return true;
				//if ( this.Inner.Count == 1 && this.Inner[ 0 ].AllowTrim )
				//    return true;
				if ( this.Inner.Count > 0 )
				{
					bool alltrim = true;
					foreach ( var inner in this.Inner )
						alltrim &= inner.AllowTrim;
					return alltrim;
				}
				return false;
			}
		}

		#region event Trimmed
		public delegate void TrimmedHandler( ParserContext value );

		private event TrimmedHandler _Trimmed;
		/// <summary>
		/// When the context was trimmed.
		/// </summary>
		public event TrimmedHandler Trimmed
		{
			add { this._Trimmed += value; }
			remove { this._Trimmed -= value; }
		}
		/// <summary>
		/// Call to raise the Trimmed event:
		/// When the context was trimmed.
		/// </summary>
		protected virtual void OnTrimmed( ParserContext value )
		{
			// if there are event subscribers...
			if ( this._Trimmed != null )
			{
				// call them...
				this._Trimmed( value );
			}
			if ( this.Outer != null )
				this.Outer.OnTrimmed( this );
		}
		#endregion event Trimmed
		#endregion Trim

		public ParserContext RootContext
		{
			get
			{
				if ( this.Outer == null )
					return this;
				else
					return this.Outer.RootContext;
			}
		}

		public const int SnippetLength = 80;
		public override string ToString()
		{
			return this.ToString( SnippetLength );
		}

		public string ToString( int snippetLength )
		{
			if ( this.Source.Length <= this.Index )
			{
				return "EOF";
			}
			else if ( this.Source.Length <= this.Index + snippetLength )
			{
				return "[...]" + this.Source.Substring( this.Index );
			}
			else
			{
				return "[...]" + this.Source.Substring( this.Index, snippetLength ) + "[...]";
			}

		}
		#endregion Methods
	}
}
