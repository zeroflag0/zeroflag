using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Imaging.Strategies
{
	public class Change : Strategy
	{
		public override Color Apply(int x, int y, Color value)
		{
			Color prev = this.Previous[x, y];
			this.Previous[x, y] = value;
			if (prev != null)
			{
				value = (value - prev) * 10f;
			}
			else
				value = new Color(0f, 0f, 0f, 0f);
			return value;
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
			return new Color[this.PixelSource.Width, this.PixelSource.Height];
		}

	}
}
