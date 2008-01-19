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

namespace zeroflag.Serialization.StringConverters
{
	public abstract class Base
	{
		#region Static
		static Dictionary<Type, Base> _Converters = null;

		public static Dictionary<Type, Base> Converters
		{
			get
			{
				if (_Converters == null)
				{
					_Converters = new Dictionary<Type, Base>();
					List<Type> types = TypeHelper.GetDerived(typeof(Base<>));
					foreach (Type type in types)
					{
						try
						{
							Base conv = (Base)TypeHelper.CreateInstance(type);
							_Converters.Add(conv.Type, conv);
						}
						catch
						{
						}
					}
				}
				return Base._Converters;
			}
		}

		static Base GetConverter(Type type)
		{
			if (type == null)
				return new Fallback();
			if (!Converters.ContainsKey(type))
			{
				Converters.Add(type, GetConverter(type.BaseType));
			}
			return Converters[type];
		}

		public static string Write(object value)
		{
			return GetConverter(value.GetType()).WriteBase(value);
		}

		public static object Parse(Type type, string text)
		{
			return GetConverter(type).ParseBase(text);
		}
		#endregion

		public abstract Type Type { get; }

		public abstract string WriteBase(object value);

		public abstract object ParseBase(string text);
	}
	public abstract class Base<T> : Base
	{
		public override Type Type
		{
			get { return typeof(T); }
		}

		public abstract string Write(T value);

		public abstract T Parse(string text);



		public override string WriteBase(object value)
		{
			return this.Write((T)value);
		}

		public override object ParseBase(string text)
		{
			return (T)this.Parse(text);
		}
	}
}
