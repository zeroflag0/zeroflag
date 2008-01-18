using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Imaging.Strategies
{
	public class Edge1 : StrategyRight
	{
		const float factor = 6.0f;

		public override Color Apply(int x, int y, Color value, Color right)
		{
			return new Color(Math.Abs(value.R - right.R) * factor, Math.Abs(value.G - right.G) * factor, Math.Abs(value.B - right.B) * factor);
		}
	}
}
