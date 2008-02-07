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
using Base = System.Drawing.Color;

namespace zeroflag.Imaging
{
	public class Color
	{
		#region Base
		Base? _Base = null;

		protected Base Base
		{
			get { return (Base)(_Base ?? (_Base = NewBase)); }
			set { _Base = value; }
		}

		protected Base? NewBase
		{
			get
			{
				return Base.FromArgb((int)(this.A * 255f), (int)(this.R * 255f), (int)(this.G * 255f), (int)(this.B * 255f));
			}
		}

		public static implicit operator Base(Color color)
		{
			return color != null ? color.Base : default(Base);
		}

		public static implicit operator Color(Base color)
		{
			return new Color(color);
		}
		public static implicit operator Color(float value)
		{
			return new Color(value);
		}
		public static implicit operator float(Color color)
		{
			return color != null ? color.Brightness : 0f;
		}
		#endregion Base

		#region Operations

		#region Delta -
		public Color Delta(Color other)
		{
			return new Color(Math.Abs(this.R - other.R), Math.Abs(this.G - other.G), Math.Abs(this.B - other.B));
		}

		public static Color operator -(Color first, Color second)
		{
			if (first != null && second != null)
				return first.Delta(second);
			else
				return new Color();
		}

		//public Color Delta(float other)
		//{
		//    return new Color((this.R - other) / 2f + 0.5f, (this.G - other) / 2f + 0.5f, (this.B - other) / 2f + 0.5f);
		//}

		//public static Color operator -(Color first, float second)
		//{
		//    if (first != null)
		//        return first.Delta(second);
		//    else
		//        return new Color();
		//}
		#endregion Delta -

		#region Amplify *
		public Color Amplify(float factor)
		{
			return new Color(this.R * factor, this.G * factor, this.B * factor);
		}

		public static Color operator *(Color color, float factor)
		{
			if (color != null)
				return color.Amplify(factor);
			else
				return new Color(Normalize(factor));
		}

		#endregion Amplify *

		#region Max
		public static Color Max(Color a, Color b)
		{
			if (b == null)
				return a;
			if (a == null)
				return b;
			if (a.Brightness > b.Brightness)
				return a;
			else
				return b;
		}

		public static Color Max(params Color[] c)
		{
			if (c.Length < 1)
				return new Color();
			if (c.Length == 1)
				return c[0];
			Color r = c[0];
			for (int i = 1; i < c.Length; i++)
			{
				r = Max(r, c[i]);
			}
			return r;
		}
		#endregion

		#endregion Operations

		#region Values

		static float Normalize(float value)
		{
			return value < 0f
				? 0f
				: value > 1f
					? 1f
					: value;
		}

		public Color Set(float r, float g, float b)
		{
			return this.Set(1.0f, r, g, b);
		}
		public Color Set(float a, float r, float g, float b)
		{
			this.A = a;
			this.R = r;
			this.G = g;
			this.B = b;
			return this;
		}

		#region A

		private float _A = default(float);

		public float A
		{
			get { return _A; }
			set
			{
				if (_A != value)
				{
					_A = Normalize(value);
				}
			}
		}

		public float Alpha
		{
			get { return this.A; }
			set { this.A = value; }
		}
		#endregion A

		#region R

		private float _R = default(float);

		public float R
		{
			get { return _R; }
			set
			{
				if (_R != value)
				{
					_R = Normalize(value);
					this._Base = null;
				}
			}
		}

		public float Red
		{
			get { return this.R; }
			set { this.R = value; }
		}
		#endregion R

		#region G

		private float _G = default(float);

		public float G
		{
			get { return _G; }
			set
			{
				if (_G != value)
				{
					_G = Normalize(value);
					this._Base = null;
				}
			}
		}

		public float Green
		{
			get { return this.G; }
			set { this.G = value; }
		}
		#endregion G

		#region B

		private float _B = default(float);

		public float B
		{
			get { return _B; }
			set
			{
				if (_B != value)
				{
					_B = Normalize(value);
					this._Base = null;
				}
			}
		}

		public float Blue
		{
			get { return this.B; }
			set { this.B = value; }
		}
		#endregion B

		#region Brightness
		/// <summary>
		/// The brightness of this Color. The brightness ranges from 0.0 through 1.0, where 0.0 represents black and 1.0 represents white. 
		/// </summary>
		public float Brightness
		{
			get { return this.Base.GetBrightness(); }
		}

		#endregion Brightness

		#region Hue
		/// <summary>
		/// The hue, in degrees, of this Color. The hue is measured in degrees, ranging from 0.0 through 360.0, in HSB color space. 
		/// </summary>
		public Hue Hue
		{
			get { return this.Base.GetHue(); }
			set { value.ApplyTo(this); }
		}
		#endregion Hue

		#region Saturation
		/// <summary>
		/// The saturation of this Color. The saturation ranges from 0.0 through 1.0, where 0.0 is grayscale and 1.0 is the most saturated.
		/// </summary>
		public float Saturation
		{
			get { return this.Base.GetSaturation(); }
		}
		#endregion Saturation

		#endregion Values

		#region Constructor
		public Color()
		{
		}

		public Color( Base color)
			: this(color.A, color.R, color.G , color.B)
		{
			this.Base = color;
		}

		protected Color(float value)
			: this(value, value , value)
		{
		}

		public Color(int r , int g, int b)
			: this(0xff, r, g , b)
		{
		}

		public Color(int a, int r, int g, int b)
			: this( (float)a / 255f, (float)r / 255f, (float)g / 255f, (float)b / 255f)
		{
		}

		public Color(float r , float g , float b)
			: this(1, r, g, b)
		{
		}

		public Color(float a, float r, float g, float b)
		{
			this.A = a;
			this.R = r;
			this.G = g;
			this.B = b;
		}
		#endregion Constructor

		public override string ToString()
		{
			return new System.Text.StringBuilder("{A=").Append(this.A.ToString("0.000"))
				.Append(",R=").Append(this.R.ToString("0.000"))
				.Append(",G=").Append(this.G.ToString("0.000"))
				.Append(",B=").Append(this.B.ToString("0.000"))
				.Append("}")
				.Append("{hue=").Append(this.Hue.Value.ToString("0.000"))
				.Append(",sat=").Append(this.Saturation.ToString("0.000"))
				.Append(",val=").Append(this.Brightness.ToString("0.000"))
				.Append("}")
				.ToString();
		}
	}
}
