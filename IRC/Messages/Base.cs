using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.IRC.Messages
{
	public abstract class Base : zeroflag.IRC.Messages.IBase
	{
		public Base()
		{
		}
		public Base(string value)
		{
			this.Parse(value);
		}

		public virtual string Generate()
		{
			return this.Generate(new StringBuilder()).ToString();
		}

		public abstract StringBuilder Generate(StringBuilder builder);

		public abstract void Parse(string value);
	}
}
