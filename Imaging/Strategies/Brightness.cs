using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Imaging.Strategies
{
	public class Brightness : Strategy
	{
		public override void PreApply()
		{
		}

		public override Color Apply(int x, int y, Color value)
		{
			return value.Brightness;
		}

		public override void PostApply()
		{
		}
	}
}
