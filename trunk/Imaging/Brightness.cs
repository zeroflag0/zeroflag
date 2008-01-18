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
