using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Imaging.Strategies
{
	public abstract class Strategy : zeroflag.Imaging.Strategies.IStrategy
	{
		public virtual void PreApply() { }

		public virtual Color Apply(int x, int y)
		{
			return (_Delegate ?? (_Delegate = this.Delegate))(x, y, this.PixelSource[x, y]);
		}

		public abstract Color Apply(int x, int y, Color value);

		public virtual void PostApply() { }

		IPixelSource _PixelSource;

		public IPixelSource PixelSource
		{
			get { return _PixelSource; }
			set { _PixelSource = value; }
		}

		public delegate Color ApplyHandler(int x, int y, Color value);

		ApplyHandler _Delegate;
		public ApplyHandler Delegate
		{
			get
			{
				if (this.Next.Count > 0)
				{
					ApplyHandler next = this.Next[0].Delegate;
					for (int i = 0; i < this.Next.Count; i++)
						next += this.Next[1].Delegate;

					return
						delegate(int x, int y, Color value)
						{
							return next(x, y, this.Apply(x, y, value));
						};
				}
				else
					return this.Apply;
			}
		}

		public virtual void Prepare()
		{
			this._Delegate = this.Delegate;
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

		public System.Drawing.Rectangle MinimumPadding
		{
			get { return new System.Drawing.Rectangle(0, 0, 0, 0); }
		}

	}
}
