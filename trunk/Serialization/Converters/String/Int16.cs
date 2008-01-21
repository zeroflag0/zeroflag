using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class Int16 : Converter<System.Int16>
	{
		public override short ___Set(string value)
		{
			return System.Int16.Parse(value);
		}
	}
}
