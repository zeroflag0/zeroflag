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
	public class Dummy : Filter
	{
		public Dummy(Filter parent, float stepx, float stepy)
			: base(parent, stepx, stepy)
		{
		}

		public Dummy()
			: base()
		{
		}

		public Dummy(Filter parent)
			: base(parent)
		{
		}

		public Dummy(Filter parent, float step)
			: base(parent, step)
		{
		}

		public Dummy(Filter parent, System.Drawing.SizeF step)
			: base(parent, step)
		{
		}

		public Dummy(Filter parent, System.Drawing.Rectangle region)
			: base(parent, region)
		{
		}

		protected override void DoRender(System.Drawing.Graphics g)
		{
			//base.DoRender(g);
			if (this.Parent != null)
			{
				//g.DrawRectangle(System.Drawing.Pens.Orange, 0, 0, this.Width * 2, this.Height * 2);

				for (int y = this.Padding.Y; y < this.Height - this.Padding.Height; y++)
				{
					for (int x = this.Padding.X; x < this.Width - this.Padding.Width; x++)
					{
						Color color = this.Parent[x + this.X, y + this.Y];
						g.FillRectangle(new System.Drawing.SolidBrush(color), x * 2, y * 2, 2, 2);
					}
				}
				g.DrawRectangle(System.Drawing.Pens.Orange, -1, -1, this.Width * 2 + 1, this.Height * 2 + 1);
			}
		}
	}
}
