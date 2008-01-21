using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters
{
	public abstract class Converter<T1, T2> : zeroflag.Serialization.Converters.IConverter
	{
		public static T1 Get(T2 value)
		{
			return (T1)GetConverter().__Get(value);
		}

		public static T2 Set(T1 value)
		{
			return (T2)GetConverter().__Set(value);
		}

		static Converter()
		{
			Type t1, t2;
			_Converters = new Dictionary<Type, Dictionary<Type, IConverter>>();

			List<Type> types = TypeHelper.GetDerived(typeof(Converter<,>));

			foreach (Type type in types)
			{
				if (type.IsAbstract || type.IsInterface)
					continue;
				IConverter converter = (IConverter)TypeHelper.CreateInstance(type);
				t1 = converter.Type1;
				t2 = converter.Type2;

				if (!_Converters.ContainsKey(t1))
					_Converters.Add(t1, new Dictionary<Type, IConverter>());

				_Converters[t1].Add(t2, converter);
			}
		}

		static Dictionary<Type, Dictionary<Type, IConverter>> _Converters;
		public static IConverter GetConverter()
		{
			return GetConverter(typeof(T1), typeof(T2));
		}

		public static IConverter GetConverter(Type t1, Type t2)
		{
			//if (!_Converters.ContainsKey(t1))
			//{
			//    return GetConverter(t1.BaseType, t2);
			//}
			//else 
			if (!_Converters[t1].ContainsKey(t2))
			{
				_Converters[t1].Add(t2, GetConverter(t1, t2.BaseType));
			}
			return (IConverter)_Converters[t1][t2];
		}


		public Type Type1 { get { return typeof(T1); } }
		public Type Type2 { get { return typeof(T2); } }

		public abstract T1 _Get(T2 value);

		public abstract T2 _Set(T1 value);

		public object __Get(object value)
		{
			return (T1)_Get((T2)value);
		}

		public object __Set(object value)
		{
			return (T2)_Set((T1)value);
		}
	}
}
