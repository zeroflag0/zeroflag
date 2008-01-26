using System;
using System.Collections.Generic;
using System.Text;
using Base = zeroflag.Serialization.Converters.Converter;
using IBase = zeroflag.Serialization.Converters.IConverter;

namespace zeroflag.Serialization.Converters.String
{
	public class Converter
	{
		public const string NullToken = "~!~null~!~";

		public static string Get(object value)
		{
			if (value == null)
				return NullToken;
			IBase b = Base.GetConverter(typeof(string), value.GetType());
			return (string)b.__Get(value);
		}
	}
}
