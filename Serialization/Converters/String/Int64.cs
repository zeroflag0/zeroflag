using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class Int64 : Converter<System.Int64>
	{
		public override System.Int64 ___Set(string value)
		{
			return System.Int64.Parse(value);
		}
	}
}
