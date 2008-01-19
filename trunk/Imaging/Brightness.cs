#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2007  Thomas "zeroflag" Kraemer
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

namespace zeroflag.Imaging
{
	public class Brightness : Filter
	{
		protected override void ApplyPixel(int x, int y)
		{
			this[x, y] = this[x, y].Brightness;
		}

		public Brightness(Filter parent, float step)
			: base(parent, step)
		{
		}

		public Brightness(Filter parent, float stepx, float stepy)
			: base(parent, stepx, stepy)
		{
		}

		public Brightness(Filter parent, System.Drawing.SizeF step)
			: base(parent,step)
		{
		}

		public Brightness()
			: base()
		{
		}

		public Brightness(Filter parent)
			: base(parent)
		{
		}

		public Brightness(Filter parent, System.Drawing.Rectangle region)
			: base(parent, region)
		{
		}
	}
}
