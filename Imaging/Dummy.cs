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
