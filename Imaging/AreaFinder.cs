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
using point = System.Drawing.Point;

namespace zeroflag.Imaging
{
	public class AreaFinder : Filter
	{
		protected override void ApplyPixel(int x, int y)
		{
			//if (this._Count++ > this.Limit)
			//{
			//    _Limit = new point(x, y);
			//    return;
			//}

			if (this.Scanned[x, y])
				return;
			//this.Scanned[x, y] = true;
			//this.Scanned.Add(p);

			Color c = this[x, y];
			if (c > this.Threashold * 1.2f)
			{
				//Console.WriteLine("Initializing area scan at " + x + "," + y);
				//Console.WriteLine();
				Region reg = new Region(x, y);
				this.ProcessPixel(x, y, reg);
				this.Results.Add(reg);
			}
		}

		bool ProcessPixel(int x, int y, Region region)
		{
			//if (this._Count++ > this.Limit)
			//{
			//    _Limit = new point(x, y);
			//    return false;
			//}
			if (x < this.Padding.X || x >= this.Width - this.Padding.Width || y < this.Padding.Y || y >= this.Height - this.Padding.Height)
				return false;
			point p = new point(x, y);
			if (this.Scanned[x, y])
				return false;
			this.Scanned[x, y] = true;

			Color c = this[x, y];
			if (c > this.Threashold)
			{
				region.Add(x, y);
				//Console.Write("Scan " + p + " Region = " + region.ToString().PadLeft(50) + "\n");


				//for (int i = this.Range; i > 0; i -= 1)
				//    if (this.ProcessPixel(x, y - i, region))
				//        break;
				//for (int i = this.Range; i > 0; i -= 1)
				//    if (this.ProcessPixel(x - i, y, region))
				//        break;
				for (int i = this.Range; i > 0; i--)
					if (this.ProcessPixel(x, y + i, region)) break;

				for (int i = this.Range; i > 0; i--)
					if (this.ProcessPixel(x + i, y, region)) break;

				for (int i = this.Range; i > 0; i--)
					if (this.ProcessPixel(x + i, y + i, region)) break;

				for (int iy = y; iy < region.Y + region.Height; iy++)
				{
					for (int ix = x; ix < region.X + region.Width; ix++)
					{
						this.Scanned[ix, iy] = true;
					}
				}
				//for (int i = this.Range; i > 0; i -= 1)
				//// scan outwards...
				//{
				//    // top, right...
				//    //this.ProcessPixel(x - i, y - i, ref region);
				//    this.ProcessPixel(x, y - i, region)
				//    //this.ProcessPixel(x + i, y - i, ref region);
				//    // left, down...
				//    this.ProcessPixel(x - i, y, region) ||
				//    //this.ProcessPixel(x - i, y + i, ref region);
				//    // bottom, right...
				//    this.ProcessPixel(x, y + i, region) ||
				//    //this.ProcessPixel(x + i, y + i, ref region);
				//    // right, down...
				//    this.ProcessPixel(x + i, y, region) 
				//    //this.ProcessPixel(x + i, y + i, ref region);
				//}
				return true;
			}
			return false;
		}

		//protected virtual point Follow(point current, point previous)
		//{
		//    if (this.Scanned.Contains(current))
		//        return current;

		//}
		protected override void DoRender(System.Drawing.Graphics g)
		{
			//g.FillRectangle(new System.Drawing.SolidBrush(new Color(0.2f, 0f, 1f, 1f)), this);
			int g1 = this.Width * this.Height / 50;
			int g2 = g1 / 50;
			int g3 = g2 / 50;
			//base.DoRender(g);
			foreach (Region region in this.Results)
			{
				if (region.Volume > g1)
					g.DrawRectangle(System.Drawing.Pens.OrangeRed, region.X, region.Y, region.Width, region.Height);
				else if (region.Volume > g2)
					g.DrawRectangle(System.Drawing.Pens.Orange, region.X, region.Y, region.Width, region.Height);
				else if (region.Volume > g3)
					g.DrawRectangle(System.Drawing.Pens.Yellow, region.X, region.Y, region.Width, region.Height);
				else
					g.DrawRectangle(System.Drawing.Pens.White, region.X, region.Y, region.Width, region.Height);
			}
			if (_Limit.X >= 0 && _Limit.Y >= 0)
			{
				g.FillRectangle(System.Drawing.Brushes.Red, _Limit.X - 5, _Limit.Y - 5, 10, 10);
			}

		}


		protected override void DoUpdate()
		{
			this._Count = 0;
			this.Scanned = new bool[this.Width, this.Height];
			this.Results.Clear();

			base.DoUpdate();
		}

		int _Range = 5;

		public int Range
		{
			get { return _Range; }
			set { _Range = value; }
		}

		//int _Limit = 20000;
		point _Limit = new point(-1, -1);
		public int Limit
		{
			get { return this.Width * this.Height * 20; }
			//get { return _Limit; }
			//set { _Limit = value; }
		}

		int _Count = 0;

		bool[,] _Scanned = null;

		protected bool[,] Scanned
		{
			get { return _Scanned; }
			set { _Scanned = value; }
		}

		List<Region> _Results = new List<Region>();

		public List<Region> Results
		{
			get { return _Results; }
		}

		public class Region : IComparable<Region>
		{
			public Region(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}
			public int X, Y, Width = 1, Height = 1;
			public void Add(int x, int y)
			{
				if (x < this.X)
				{
					this.Width += this.X - x;
					this.X = x;
				}
				else if (x >= this.Width + this.X)
				{
					this.Width += x - this.Width - this.X + 1;
				}
				if (y < this.Y)
				{
					this.Height += this.Y - y;
					this.Y = y;
				}
				else if (y >= this.Height + this.Y)
				{
					this.Height += y - this.Height - this.Y + 1;
				}
			}
			public override string ToString()
			{
				return "{" + this.X + "," + this.Y + "}{" + this.Width + "," + this.Height + "}";
			}
			public int Volume
			{
				get
				{
					return this.Width * this.Height;
				}
			}


			#region IComparable<Region> Members

			public int CompareTo(Region other)
			{
				return other != null ? this.Volume.CompareTo(other.Volume) : this.Volume.CompareTo(0);
			}

			#endregion
		}

		#region Constructors
		public AreaFinder(Filter parent, float stepx, float stepy)
			: base( parent, stepx, stepy)
		{
		}

		public AreaFinder()
			: base()
		{
		}

		public AreaFinder(Filter parent)
			: base(parent)
		{
		}

		public AreaFinder(Filter parent, float step)
			: base(parent, step)
		{
		}

		public AreaFinder(Filter parent, System.Drawing.SizeF step)
			: base(parent, step)
		{
		}

		public AreaFinder(Filter parent, System.Drawing.Rectangle region)
			: base(parent, region)
		{
		}
		#endregion Constructors

		private float _Threashold = 0.2f;

		public float Threashold
		{
			get { return _Threashold; }
			set { _Threashold = value; }
		}
	}
}
