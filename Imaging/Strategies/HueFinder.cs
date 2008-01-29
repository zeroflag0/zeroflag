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
	public class HueFinder : Strategy
	{
		public override Color Apply(int x, int y, Color value)
		{
			//if (value.Saturation > this.MinBrightness && value.Saturation < this.MaxBrightness)
			if (value.Saturation > this.MinSaturation && value.Brightness < 0.5f)
				foreach (Hue hue in this.Hues)
					if (value.Hue == hue)
						return //(Color)(hue + 180f);
							new Color(1f, 1f, 1f);

			return new Color(0f, 0f, 0f, 0f);
			//return value.Hue == Hue ? value : null;
		}

		List<Hue> _Hues = new List<Hue>();

		public List<Hue> Hues
		{
			get { return _Hues; }
			set { _Hues = value; }
		}

		float _MinSaturation = 0.8f;

		public float MinSaturation
		{
			get { return _MinSaturation; }
			set { _MinSaturation = value; }
		}

		//float _MaxBrightness = 0.7f;

		//public float MaxBrightness
		//{
		//    get { return _MaxBrightness; }
		//    set { _MaxBrightness = value; }
		//}

		public HueFinder()
		{
		}

		public HueFinder(params Hue[] hues)
		{
			this.Hues.AddRange(hues);
		}

		public HueFinder(float minSaturation/*, float maxBrightness*/, params Hue[] hues)
			: this(hues)
		{
			this.MinSaturation = minSaturation;
			//this.MaxBrightness = maxBrightness;
		}
	}
}
