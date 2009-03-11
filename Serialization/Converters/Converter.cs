#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag >>at<< zeroflag >>dot<< de
//	
//	Copyright (C) 2006-2009  Thomas "zeroflag" Kraemer
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
#if OLD
		static ConverterCollection _Converters = new ConverterCollection();

		public static ConverterCollection Converters
		{
			get { return Converter._Converters; }
		}

		public static IConverter GetConverter(Type t1, Type t2)
		{
			return Converters.GetConverter(t1, t2);
		}
#endif
		#region IConverter Members

		public abstract object __Generate(Type type, object value);

		public abstract object __Parse(Type type, object value);

		public abstract Type Type1 { get; }

		public abstract Type Type2 { get; }

		#endregion
	}
}
