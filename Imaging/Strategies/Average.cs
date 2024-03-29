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
