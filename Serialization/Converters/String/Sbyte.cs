using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class SByte : Converter<System.SByte>
	{
		public override System.SByte ___Set(string value)
		{
			return System.SByte.Parse(value);
		}
	}
}
