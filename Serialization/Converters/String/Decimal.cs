using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class Decimal : Converter<System.Decimal>
	{
		public override System.Decimal ___Set(string value)
		{
			return System.Decimal.Parse(value);
		}
	}
}
