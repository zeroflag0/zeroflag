using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace zeroflag.IRC.Messages
{
	public abstract class RegexBase : Base
	{
		Regex _Regex;

		public Regex Regex
		{
			get { return _Regex ?? (_Regex = this.CreateRegex); }
		}
		protected abstract Regex CreateRegex
		{
			get;
		}

		public override void Parse(string value)
		{
			this.ParseAssign(this.Regex.Match(value));
		}

		public abstract void ParseAssign(Match match);

		public RegexBase()
		{
		}

		public RegexBase(string value)
			: base(value)
		{
		}
	}
}
