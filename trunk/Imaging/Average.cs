using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Imaging
{
	public class Average : StrategyFilter
	{
		public Average(Filter parent, float stepx, float stepy)
			: base(parent, stepx, stepy)
		{
		}

		public Average()
			: base()
		{
		}

		public Average(Filter parent)
			: base(parent)
		{
		}

		public Average(Filter parent, float step)
			: base(parent, step)
		{
		}

		public Average(Filter parent, System.Drawing.SizeF step)
			: base(parent, step)
		{
		}

		public Average(Filter parent, System.Drawing.Rectangle region)
			: base(parent, region)
		{
		}

		public Color Result
		{
			get { return ((Strategies.Average)this.Strategy).Result; }
		}

		public override zeroflag.Imaging.Strategies.IStrategy CreateStrategy()
		{
#if DEBUG
			return new Strategies.AveragePaint();
#else
			return new Strategies.Average();
#endif
			//Strategies.Average av = new Strategies.Average();
			//av.Then(new Strategies.AveragePaint());
			//return av;
		}
	}
}
