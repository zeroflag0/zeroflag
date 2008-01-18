using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Imaging.Strategies
{
	public abstract class StrategyRight : zeroflag.Imaging.Strategies.IStrategy
	{
		public virtual void PreApply() { }

		public abstract Color Apply(int x, int y, Color value, Color right);

		public virtual void PostApply() { }


		public delegate Color ApplyHandler(int x, int y, Color value, Color right);

		public ApplyHandler Delegate
		{
			get
			{
				if (this.Next.Count > 0)
				{
					Strategy.ApplyHandler next = this.Next[0].Delegate;
					for (int i = 0; i < this.Next.Count; i++)
						next += this.Next[1].Delegate;

					return
						delegate(int x, int y, Color value, Color right)
						{
							return next(x, y, this.Apply(x, y, value, right));
						};
				}
				else
					return this.Apply;
			}
		}

		List<Strategy> _Next = new List<Strategy>();

		public List<Strategy> Next
		{
			get { return _Next; }
		}

		public Strategies.Strategy Then(Strategies.Strategy next)
		{
			this.Next.Add(next);
			return next;
		}

		#region IStrategy Members

		ApplyHandler _Delegate;

		public Color Apply(int x, int y)
		{
			return (_Delegate ?? (_Delegate = this.Delegate))(x, y, this.PixelSource[x, y], this.PixelSource[x + 1, y]);
		}

		public void Prepare()
		{
			_Delegate = this.Delegate;
		}

		IPixelSource _PixelSource;

		public IPixelSource PixelSource
		{
			get { return _PixelSource; }
			set { _PixelSource = value; }
		}

		public System.Drawing.Rectangle MinimumPadding
		{
			get { return new System.Drawing.Rectangle(0, 0, 1, 0); }
		}
		#endregion
	}
}
