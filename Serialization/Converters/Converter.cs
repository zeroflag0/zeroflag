#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2008  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************
#endregion LGPL License

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

				_Converters[t1][t2] = converter;
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
				return null;
			if (!_Converters[t1].ContainsKey(t2))
			{
				IConverter conv = GetConverter(t1, t2.BaseType);
				if (conv != null)
					_Converters[t1].Add(t2, conv);
				else
					return null;
			}
			return (IConverter)_Converters[t1][t2];
		}


		#region IConverter Members

		public abstract object __Generate(Type type, object value);

		public abstract object __Parse(Type type, object value);

		public abstract Type Type1 { get; }

		public abstract Type Type2 { get; }

		#endregion
	}
}
