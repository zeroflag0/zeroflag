using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.StringConverters
{
	public class Fallback : Base
	{
		public override Type Type
		{
			get { return null; }
		}

		public override string WriteBase(object value)
		{
			return value != null ? value.ToString() : "";
		}

		public override object ParseBase(string text)
		{
			return null;
		}
	}
}
