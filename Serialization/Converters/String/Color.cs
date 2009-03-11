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

namespace zeroflag.Serialization.Converters.String
{
	public class Color : Converter<System.Drawing.Color>
	{
		public override System.Drawing.Color ___Parse(Type type, string value)
		{
			System.Drawing.Color result = System.Drawing.Color.Empty;
			try
			{
				result = System.Drawing.Color.FromName(value);
			}
			catch
			{
			}
			if (result.IsEmpty || (!result.IsKnownColor && result.IsNamedColor && result.A == 0 && result.R == 0 && result.B == 0 && result.G == 0))
			{
				value = value.Trim().TrimStart('#');
				try
				{
					result = System.Drawing.Color.FromArgb(255, System.Drawing.Color.FromArgb(int.Parse(value, System.Globalization.NumberStyles.HexNumber)));
				}
				catch
				{
					result = System.Drawing.Color.FromArgb(255, System.Drawing.Color.FromArgb(int.Parse(value)));
				}
			}
			return result;
		}
	}
}
