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

namespace zeroflag.Imaging
{
	public class EdgeBW : Filter
	{
		//protected override void Apply()
		//{
		//    for (int y = this.Padding.Y; y < this.Height - this.Padding.Height; y++)
		//    {
		//        for (int x = this.Padding.X; x < this.Width - this.Padding.Width; x++)
		//        {
		//        }
		//    }
		//}

		protected override void ApplyPixel(int x, int y)
		{
			//float value = this[x, y];
			//float dx = this[x + 1, y];
			//float dy = this[x, y + 1];
			//float dxy = this[x + 1, y + 1];
			//dx -= value;
			//dy -= value;
			//dxy -= value;
			//value = Math.Max(Math.Max(Math.Abs(dx), Math.Abs(dy)), Math.Abs(dxy));
			////value = Color.Max(dx, dy, dxy);
			//this[x, y] = value * 1.2f;

			float r, g, b, v;
			Color value = this[x, y];
			Color dx = this[x + 1, y];
			Color dy = this[x, y + 1];

			r = Math.Abs(value.R - dx.R) + Math.Abs(value.R - dy.R);
			g = Math.Abs(value.G - dx.G) + Math.Abs(value.G - dy.G);
			b = Math.Abs(value.B - dx.B) + Math.Abs(value.B - dy.B);
			v = (r + g + b) * 1.5f;
			this[x, y] = v;
		}

		public override System.Drawing.Rectangle Padding
		{
			get
			{
				return new System.Drawing.Rectangle(0, 0, 1, 1);
			}
			set
			{
			}
		}

		#region Constructors
		public EdgeBW(Filter parent, float stepx, float stepy)
			: base(parent, stepx, stepy)
		{
		}

		public EdgeBW()
			: base()
		{
		}

		public EdgeBW(Filter parent)
			: base(parent)
		{
		}

		public EdgeBW(Filter parent, float step)
			: base(parent, step)
		{
		}

		public EdgeBW(Filter parent, System.Drawing.SizeF step)
			: base(parent, step)
		{
		}

		public EdgeBW(Filter parent, System.Drawing.Rectangle region)
			: base(parent, region)
		{
		}
		#endregion Constructors
	}
}
