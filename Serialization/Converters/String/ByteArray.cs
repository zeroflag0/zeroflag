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

namespace zeroflag.Serialization.Converters.String
{
	public class ByteArray : Converter<System.Byte[]>
	{
		public override System.Byte[] ___Parse( Type type, string value )
		{
			if ( value.ToLower().StartsWith( "0x" ) )
				value = value.Remove( 0, 2 );
			List<byte> result = new List<byte>();
			for ( int i = 0; i < value.Length; i += 2 )
			{
				result.Add( System.Byte.Parse( value.Substring( i, 2 ) ) );
			}
			return result.ToArray();
		}

		public override string _Generate( Type type, byte[] value )
		{
			StringBuilder result = new StringBuilder( "0x" );
			foreach ( byte b in value )
			{
				result.Append( b.ToString( "X" ).PadLeft( 2, '0' ) );
			}
			return result.ToString();
		}
	}
}
