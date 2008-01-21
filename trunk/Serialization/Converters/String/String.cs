using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class String : Converter<System.String>
	{
		public override System.String ___Set(string value)
		{
			return value;
		}
	}
}
