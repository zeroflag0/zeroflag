using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class Double : Converter<System.Double>
	{
		public override System.Double ___Set(string value)
		{
			return System.Double.Parse(value);
		}
	}
}
