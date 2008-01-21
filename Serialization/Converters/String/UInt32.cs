using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class UInt32 : Converter<System.UInt32>
	{
		public override System.UInt32 ___Set(string value)
		{
			return System.UInt32.Parse(value);
		}
	}
}
