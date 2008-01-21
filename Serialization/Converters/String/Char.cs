using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class Char : Converter<System.Char>
	{
		public override System.Char ___Set(string value)
		{
			return System.Char.Parse(value);
		}
	}
}
