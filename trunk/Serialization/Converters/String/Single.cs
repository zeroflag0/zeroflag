using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class Single : Converter<System.Single>
	{
		public override System.Single ___Set(string value)
		{
			return System.Single.Parse(value);
		}
	}
}
