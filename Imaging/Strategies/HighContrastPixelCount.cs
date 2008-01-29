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
	public class HighContrastPixelCount : Strategy
	{
		public override Color Apply(int x, int y, Color value)
		{
			//if (value.R > this.Contrast.R && value.G > this.Contrast.G && value.B > this.Contrast.B)
			double a = value.Hue;
			a = Math.Abs(a - h);
			if (a > 180f)
				a = 360f - a;
			a /= 180f;
			if (a < this.Threashold && Math.Abs(value.Brightness - b) < this.Threashold)
				v++;
			c++;

			return value;
		}

		double h, b;
		long v, c;

		float _Result;

		public float Result
		{
			get { return _Result; }
			set { _Result = value; }
		}

		Color _Contrast = new Color(0.5f, 0.5f, 0.5f);

		public Color Contrast
		{
			get { return _Contrast; }
			set { _Contrast = value; }
		}
		float _Threashold = 0.5f;

		public float Threashold
		{
			get { return _Threashold; }
			set { _Threashold = value; }
		}

		public HighContrastPixelCount(Color contrast, float threashold)
		{
			this.Contrast = contrast;
			this.Threashold = threashold;
		}

		public override void PreApply()
		{
			v = c = 0;
			b = this.Contrast.Brightness;
			h = this.Contrast.Hue;
			base.PreApply();
		}

		public override void PostApply()
		{
			if (c > 0)
				this.Result = (float)v / (float)c;
			else
				this.Result = 0;

			//foreach (Strategy strat in this.Next)
			//{
			//    if (strat is AveragePaint)
			//        ((AveragePaint)strat).Value = new Color(0.5f, this.Result.R, this.Result.G, this.Result.B);
			//}
			base.PostApply();
		}

		public override string ToString()
		{
			return base.GetType().Name + " " + this.Result + " v=" + v + " c=" + c;
		}
	}
}
