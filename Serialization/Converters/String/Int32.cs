using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class Int32 : Converter<System.Int32>
	{
		public override System.Int32 ___Set(string value)
		{
			return System.Int32.Parse(value);
		}
	}
}
