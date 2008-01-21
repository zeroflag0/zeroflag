using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class UInt16 : Converter<System.UInt16>
	{
		public override System.UInt16 ___Set(string value)
		{
			return System.UInt16.Parse(value);
		}
	}
}
