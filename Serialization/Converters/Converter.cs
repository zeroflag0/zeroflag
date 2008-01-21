using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters
{
	public abstract class Converter<T1, T2>
	{
		public abstract T1 Get(T2 value);

		public abstract T2 Set(T1 value);
	}
}
