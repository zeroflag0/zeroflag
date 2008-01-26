using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters
{
	public abstract class Converter<T1, T2> : zeroflag.Serialization.Converters.Converter
	{
		public static T1 Get(T2 value)
		{
			return (T1)GetConverter().__Get(value);
		}

		public static T2 Set(T1 value)
		{
			return (T2)GetConverter().__Set(value);
		}

		public static IConverter GetConverter()
		{
			return GetConverter(typeof(T1), typeof(T2));
		}

		public override Type Type1 { get { return typeof(T1); } }
		public override Type Type2 { get { return typeof(T2); } }

		public override object __Get(object value)
		{
			return (T1)_Get((T2)value);
		}

		public override object __Set(object value)
		{
			return (T2)_Set((T1)value);
		}

		public abstract T1 _Get(T2 value);

		public abstract T2 _Set(T1 value);


	}
}
