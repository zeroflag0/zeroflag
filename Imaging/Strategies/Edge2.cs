using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Imaging.Strategies
{
	public class Edge2 : StrategyRightDown
	{
		const float factor = 12.0f;

		public override Color Apply(int x, int y, Color value, Color right, Color down)
		{
			return new Color(
				Math.Max(Math.Abs(value.R - right.R), Math.Abs(value.R - down.R)) * factor,
				Math.Max(Math.Abs(value.G - right.G), Math.Abs(value.G - down.G)) * factor,
				Math.Max(Math.Abs(value.B - right.B), Math.Abs(value.B - down.B)) * factor);
		}
	}
}
