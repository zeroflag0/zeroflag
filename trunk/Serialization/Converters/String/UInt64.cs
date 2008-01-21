using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class UInt64 : Converter<System.UInt64>
	{
		public override System.UInt64 ___Set(string value)
		{
			return System.UInt64.Parse(value);
		}
	}
}
