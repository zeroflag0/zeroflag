using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.StringConverters
{
	public abstract class Base
	{
		public abstract Type Type { get; }

		public abstract string Write(object value);

		public abstract object Parse(string value);
	}
}
