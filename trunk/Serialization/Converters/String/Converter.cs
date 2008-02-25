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
using Base = zeroflag.Serialization.Converters.Converter;
using IBase = zeroflag.Serialization.Converters.IConverter;

namespace zeroflag.Serialization.Converters.String
{
	public class Converter
	{
		//public const string NullToken = "~!~null~!~";
		public static string Generate(object value)
		{
			return value == null ? null : Generate(value.GetType(), value);
		}
		public static string Generate(Type type, object value)
		{
			if (value == null)
				return null;
			IBase b = Base.GetConverter(typeof(string), value.GetType());
			return b != null ? (string)b.__Generate(type, value) : null;
		}

		public static bool CanConvert(object value)
		{
			return value != null && Base.GetConverter(typeof(string), value.GetType()) != null;
		}

		public static object Parse(Type type, string text)
		{
			IBase b = Base.GetConverter(typeof(string), type);
			if (b != null)
			{
				return b.__Parse(type, text);
			}
			else
			{
				try
				{
					System.Reflection.MethodInfo info = type.GetMethod("Parse");
					if (info == null)
						info = type.GetMethod("parse");
					object result = info.Invoke(null, new object[] { text });
					if (!type.IsAssignableFrom(result.GetType()))
						throw new InvalidCastException();
					return result;
				}
				catch (Exception exc)
				{
					Console.WriteLine(exc);
				}
				return null;
			}
		}
	}
}
