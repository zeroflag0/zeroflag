using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Imaging
{
	public class Edge : EdgeBW
	{
		const float factor = 6.0f;

		protected override void ApplyPixel(int x, int y)
		{
			float r, g, b;
			Color value = this[x, y];
			Color dx = this[x + 1, y];
			//Color dy = this[x, y + 1];
			//Color dxy = this[x + 1, y + 1];

			//r = Math.Max(Math.Max(Math.Abs(dx.R - value.R), Math.Abs(dy.R - value.R)), Math.Abs(dxy.R - value.R));
			//g = Math.Max(Math.Max(Math.Abs(dx.G - value.G), Math.Abs(dy.G - value.G)), Math.Abs(dxy.G - value.G));
			//b = Math.Max(Math.Max(Math.Abs(dx.B - value.B), Math.Abs(dy.B - value.B)), Math.Abs(dxy.B - value.B));
			r = Math.Abs(value.R - dx.R);
			g = Math.Abs(value.G - dx.G);
			b = Math.Abs(value.B - dx.B);

			//value = Math.Max(Math.Max(Math.Abs(dx), Math.Abs(dy)), Math.Abs(dxy));
			//value = Color.Max(dx, dy, dxy);
			this[x, y] = new Color(r * factor, g * factor, b * factor);
			//this[x, y] = new Color((r - g * b) * factor, (g - r * b) * factor, (b - r * g) * factor);// *1.2f;
		}

		#region Constructors
		public Edge(Filter parent, float stepx, float stepy)
			: base(parent, stepx, stepy)
		{
		}

		public Edge()
			: base()
		{
		}

		public Edge(Filter parent)
			: base(parent)
		{
		}

		public Edge(Filter parent, float step)
			: base(parent, step)
		{
		}

		public Edge(Filter parent, System.Drawing.SizeF step)
			: base(parent, step)
		{
		}

		public Edge(Filter parent, System.Drawing.Rectangle region)
			: base(parent, region)
		{
		}
		#endregion Constructors
	}
}
