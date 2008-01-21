using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class Byte : Converter<System.Byte>
	{
		public override System.Byte ___Set(string value)
		{
			return System.Byte.Parse(value);
		}
	}
}
