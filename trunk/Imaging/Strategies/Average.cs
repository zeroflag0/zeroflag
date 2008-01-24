using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Imaging.Strategies
{
	public class Average : Strategy
	{
		public override Color Apply(int x, int y, Color value)
		{
			r += value.R;
			g += value.G;
			b += value.B;
			c++;

			return value;
		}

		double r, g, b, c;

		Color _Result;

		public Color Result
		{
			get { return _Result; }
			set { _Result = value; }
		}

		public override void PreApply()
		{
			r = 0;
			g = 0;
			b = 0;
			c = 0;
			base.PreApply();
		}

		public override void PostApply()
		{
			this.Result = new Color((float)(r / c), (float)(g / c), (float)(b / c));

			//foreach (Strategy strat in this.Next)
			//{
			//    if (strat is AveragePaint)
			//        ((AveragePaint)strat).Value = new Color(0.5f, this.Result.R, this.Result.G, this.Result.B);
			//}
			base.PostApply();
		}

		public override string ToString()
		{
			return base.GetType().Name + this.Result;
		}
	}

	public class AveragePaint : Average
	{
		public override Color Apply(int x, int y, Color value)
		{
			value = base.Apply(x, y, value);
			return this.Result ?? value;
		}
	}
}
