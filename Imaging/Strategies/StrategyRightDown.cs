#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2008  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************
#endregion LGPL License

using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Imaging.Strategies
{
	public abstract class StrategyRightDown : IStrategy
	{
		public virtual void PreApply() { }

		public abstract Color Apply(int x, int y, Color value, Color right, Color down);

		public virtual void PostApply() { }


		public delegate Color ApplyHandler(int x, int y, Color value, Color right, Color down);

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
						delegate(int x, int y, Color value, Color right, Color down)
						{
							return next(x, y, this.Apply(x, y, value, right, down));
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
			return (_Delegate ?? (_Delegate = this.Delegate))(x, y, this.PixelSource[x, y], this.PixelSource[x + 1, y], this.PixelSource[x, y + 1]);
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
			get { return new System.Drawing.Rectangle(0, 0, 1, 1); }
		}

		#endregion
	}
}