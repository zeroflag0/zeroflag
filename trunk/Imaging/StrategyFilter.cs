using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Imaging
{
	public class StrategyFilter : Filter
	{
		protected override void ApplyPixel(int x, int y)
		{
			//base.ApplyPixel(x, y);
			try
			{
				this[x, y] = this.Strategy.Apply(x, y);
			}
			catch (IndexOutOfRangeException exc)
			{
				Console.WriteLine(exc);
			}
		}

		protected override void DoUpdate()
		{
			if (this.Strategy != null)
			{
				this.Padding = new System.Drawing.Rectangle(Math.Max(this.Padding.X, this.Strategy.MinimumPadding.X), Math.Max(this.Padding.Y, this.Strategy.MinimumPadding.Y), Math.Max(this.Padding.Width, this.Strategy.MinimumPadding.Width), Math.Max(this.Padding.Height, this.Strategy.MinimumPadding.Height));
				this.Strategy.PixelSource = this;
				this.Strategy.Prepare();
				base.DoUpdate();
			}
		}


		Strategies.IStrategy _Strategy;

		public Strategies. IStrategy Strategy
		{
			get { return _Strategy; }
			set { _Strategy = value; }
		}

		public StrategyFilter Do(Strategies.IStrategy strategy)
		{
			this.Strategy = strategy;
			return this;
		}

		#region Constructors
		public StrategyFilter(Filter parent, float stepx, float stepy)
			: base(parent, stepx, stepy)
		{
		}

		public StrategyFilter()
			: base()
		{
		}

		public StrategyFilter(Filter parent)
			: base(parent)
		{
		}

		public StrategyFilter(Filter parent, float step)
			: base(parent, step)
		{
		}

		public StrategyFilter(Filter parent, System.Drawing.SizeF step)
			: base(parent, step)
		{
		}

		public StrategyFilter(Filter parent, System.Drawing.Rectangle region)
			: base(parent, region)
		{
		}
		#endregion Constructors
	}
}
