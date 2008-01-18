using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Imaging
{
	public class Change : Filter
	{
		protected override void ApplyPixel(int x, int y)
		{
			Color prev = this.Previous[x, y];
			this.Previous[x, y] = this[x, y];
			if (prev != null)
			{
				this[x, y] = this[x, y] - prev;
			}
			else
				this[x, y] = 0f;
			//this[x, y] = this[x, y];
		}

		private Color[,] _Previous = null;

		protected Color[,] Previous
		{
			get { return _Previous ?? (_Previous = this.CreatePrevious()); }
			set
			{
				if (_Previous != value)
				{
					_Previous = value;
				}
			}
		}

		protected virtual Color[,] CreatePrevious()
		{
			return new Color[this.Width, this.Height];
		}

		#region Constructors
		public Change( Filter parent, float stepx, float stepy)
			: base(parent, stepx, stepy)
		{
		}

		public Change()
			: base()
		{
		}

		public Change(Filter parent)
			: base(parent)
		{
		}

		public Change(Filter parent, float step)
			: base(parent, step)
		{
		}

		public Change(Filter parent, System.Drawing.SizeF step)
			: base(parent, step)
		{
		}

		public Change(Filter parent, System.Drawing.Rectangle region)
			: base(parent, region)
		{
		}
		#endregion Constructors
	}
}
