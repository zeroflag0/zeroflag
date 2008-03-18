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
	public abstract class Converter<T1, T2> : zeroflag.Serialization.Converters.Converter
	{
#if OLD
		public static T1 Generate(Type type, T2 value)
		{
			return (T1)GetConverter().__Generate(type, value);
		}

		public static T2 Parse(Type type, T1 value)
		{
			return (T2)GetConverter().__Parse(type, value);
		}

		public static IConverter GetConverter()
		{
			return GetConverter(typeof(T1), typeof(T2));
		}
#endif
		public override Type Type1 { get { return typeof(T1); } }
		public override Type Type2 { get { return typeof(T2); } }

		public override object __Generate(Type type, object value)
		{
			return (T1)_Generate(type, (T2)value);
		}

		public override object __Parse(Type type, object value)
		{
			return (T2)_Parse(type, (T1)value);
		}

		public abstract T1 _Generate(Type type, T2 value);

		public abstract T2 _Parse(Type type, T1 value);


	}
}
