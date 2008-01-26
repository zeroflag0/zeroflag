using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public abstract class Converter<T> : Converter<string, T>
	{
		public override string _Get(T value)
		{
			return object.ReferenceEquals(null, value) ? Converter.NullToken : value.ToString();
		}

		public override T _Set(string value)
		{
			if (value == Converter.NullToken)
				return default(T);
			else
				return ___Set(value);
		}

		public abstract T ___Set(string value);
	}
}
