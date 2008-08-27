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

namespace zeroflag.Imaging.Strategies
{
	public class Change : Strategy
	{
		public override Color Apply( int x, int y, Color value )
		{
			Color prev = this.Previous[ x, y ];
			this.Previous[ x, y ] = value;
			if ( prev != null )
			{
				value = ( value - prev ) * 10f;
			}
			else
				value = new Color( 0f, 0f, 0f, 0f );
			return value;
		}

		private Color[ , ] _Previous = null;

		protected Color[ , ] Previous
		{
			get { return _Previous ?? ( _Previous = this.CreatePrevious() ); }
			set
			{
				if ( _Previous != value )
				{
					_Previous = value;
				}
			}
		}

		protected virtual Color[ , ] CreatePrevious()
		{
			if ( this.PixelSource != null )
				return new Color[ this.PixelSource.Width, this.PixelSource.Height ];
			else
				return null;
		}

	}
}
