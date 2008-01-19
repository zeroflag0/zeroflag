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
	public class LineFinder : Filter
	{
		#region Properties
		private int _MinimumLength = 2;

		List<point> _Scanned = new List<System.Drawing.Point>();

		private float _Threashold = 0.3f;

		public int MinimumLength
		{
			get { return _MinimumLength; }
			set { _MinimumLength = value; }
		}

		protected List<point> Scanned
		{
			get { return _Scanned; }
		}

		public float Threashold
		{
			get { return _Threashold; }
			set { _Threashold = value; }
		}
		#endregion Properties

		int followed = 0;
		point? FollowLine(point previous, point current, ref Line line)
		{
			if (Scanned.Contains(current))
				return null;
			this.Scanned.Add(current);
			followed++;

			point? result = null;
			if (line == null)
			{
				line = new Line(current);
				// find any line...
				for (int y = current.Y - 1; y <= current.Y + 1; y++)
				{
					for (int x = current.X - 1; x <= current.X + 1; x++)
					{
						if (x == current.X && y == current.Y)
							continue;
						if (x >= 0 && y >= 0 && x < this.Width && y < this.Height && this[x, y] >= this.Threashold)
						{
							result = this.FollowLine(current, new point(x, y), ref line);
							if (result != null)
								break;
						}
					}
				}
			}
			else
			{
				// follow the current line...
				point[] possible = GetNext(current.X - previous.X, current.Y - previous.Y);

				result = current;
				int x, y;
				foreach (point next in possible)
				{
					if (line.IsValid(-next.X, -next.Y))
					{
						x = current.X + next.X;
						y = current.Y + next.Y;
						if (x >= 0 && y >= 0 && x < this.Width && y < this.Height && this[x, y] >= this.Threashold)
						{
							line.B = new point(x, y);
							result = FollowLine(current, new point(x, y), ref line);
							if (result != null)
								break;
						}
					}
				}
			}
			if (result != null)
				this.Scanned.Add(result.Value);
			return result;
		}

		point[] GetNext(int x, int y)
		{
			point[] results = new point[3];

			if (x == 0)
			{
				results[0] = new point(0, y);
				results[1] = new point(-1, y);
				results[2] = new point(+1, y);
			}
			else if (y == 0)
			{
				results[0] = new point(x, 0);
				results[1] = new point(x, -1);
				results[2] = new point(x, +1);
			}
			else
			{
				results[0] = new point(x, y);
				results[1] = new point(x, 0);
				results[2] = new point(0, y);
			}
			return results;
		}

		protected override void ApplyPixel(int x, int y)
		{
			if (Scanned.Contains(new point(x, y)) || this[x, y] < this.Threashold)
				return;

			Line line = null;
			this.FollowLine(new point(x, y), new point(x, y), ref line);

			if (line != null && line.Length > this.MinimumLength)
				this.Lines.Add(line);
		}

		protected override void DoUpdate()
		{
			this.Lines.Clear();
			this.Scanned.Clear();
			followed = 0;

			base.DoUpdate();

			Console.WriteLine("Found " + this.Lines.Count + " lines followed " + followed + " pixels.");
		}

		protected override void DoRender(System.Drawing.Graphics g)
		{
			base.DoRender(g);

			foreach (Line line in this.Lines)
			{
				g.DrawLine(System.Drawing.Pens.Red, line.A, line.B);
			}
		}

		//public override System.Drawing.Rectangle Padding
		//{
		//    get
		//    {
		//        return new System.Drawing.Rectangle(1, 1, 1, 1);
		//    }
		//    set
		//    {
		//    }
		//}

		#region Lines

		List<Line> _Lines = new List<Line>();

		public List<Line> Lines
		{
			get { return _Lines; }
		}

		public class Line
		{
			public Line(point a)
			{
				this.A = a;
				this.B = a;
			}
			public Line(System.Drawing.Point a, System.Drawing.Point b, float value)
			{
				this.A = a;
				this.B = b;
				this.Value = value;
			}
			public System.Drawing.Point A, B;
			public float Value;
			public int Length { get { return (int)Math.Sqrt((A.X - B.X) * (A.X - B.X) + (A.Y - B.Y) * (A.Y - B.Y)); } }

			public bool IsValid(int ix, int iy)
			{
				if (this.Length == 0)
					return true;
				float x = ix;// -A.X;
				float y = iy;// -A.X;
				float dx = (float)(A.X - B.X);
				float dy = (float)(A.Y - B.Y);
				float l = (float)this.Length;
				dx /= l;
				dy /= l;
				return Math.Abs(x - dx) < 1 && Math.Abs(y - dy) < 1;
			}
		}
		#endregion Lines

		#region Constructors
		public LineFinder()
			: base()
		{
		}

		public LineFinder(Filter parent)
			: base(parent)
		{
		}

		public LineFinder(Filter parent, float step)
			: base(parent, step)
		{
		}

		public LineFinder(Filter parent, System.Drawing.SizeF step)
			: base(parent, step)
		{
		}

		public LineFinder(Filter parent, float stepx, float stepy)
			: base(parent, stepx, stepy)
		{
		}

		public LineFinder(Filter parent, System.Drawing.Rectangle region)
			: base(parent, region)
		{
		}


		#endregion
	}
}
