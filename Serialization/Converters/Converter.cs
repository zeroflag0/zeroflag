using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters
{
	public abstract class Converter : zeroflag.Serialization.Converters.IConverter
	{

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

		public static IConverter GetConverter(Type t1, Type t2)
		{
			//if (!_Converters.ContainsKey(t1))
			//{
			//    return GetConverter(t1.BaseType, t2);
			//}
			//else 
			if (t2 == null)
				t2 = typeof(object);
			if (!_Converters[t1].ContainsKey(t2))
			{
				_Converters[t1].Add(t2, GetConverter(t1, t2.BaseType));
			}
			return (IConverter)_Converters[t1][t2];
		}


		#region IConverter Members

		public abstract object __Get(object value);

		public abstract object __Set(object value);

		public abstract Type Type1 { get;}

		public abstract Type Type2 { get;}

		#endregion
	}
}
