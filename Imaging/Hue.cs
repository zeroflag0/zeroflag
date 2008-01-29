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

namespace zeroflag.Imaging
{
	public struct Hue : IComparable, IComparable<float>, IComparable<Hue>, IComparable<Color>
	{
		#region Constructors
		public Hue(float value)
		{
			_Value = value;
			_Epsilon = EpsilonDefault;
		}
		public Hue(float value, float epsilon)
		{
			_Value = value;
			_Epsilon = epsilon;
		}

		public Hue(Hue value, float epsilon)
		{
			_Value = value.Value;
			_Epsilon = epsilon;
		}

		public Hue(Color color)
		{
			_Value = color.Hue;
			_Epsilon = EpsilonDefault;
		}
		#endregion

		#region Predefined Colors
		public static readonly Hue Red = new Hue(0f);
		public static readonly Hue Orange = new Hue(30f);

		public static readonly Hue Yellow = new Hue(60f);
		public static readonly Hue LimeGreen = new Hue(90f);

		public static readonly Hue Green = new Hue(120f);
		public static readonly Hue Teal = new Hue(150f);

		public static readonly Hue Cyan = new Hue(180f);
		public static readonly Hue AzureBlue = new Hue(210f);

		public static readonly Hue Blue = new Hue(240f);
		public static readonly Hue Amethyst = new Hue(270f);

		public static readonly Hue Pink = new Hue(300f);
		public static readonly Hue RoseRed = new Hue(330f);
		#endregion

		#region Casts
		public static implicit operator float(Hue value)
		{
			return value.Value;
		}

		public static implicit operator Hue(float value)
		{
			return new Hue(value);
		}

		public static implicit operator Hue(Color color)
		{
			return new Hue(color);
		}

		public static explicit operator Color(Hue hue)
		{
			return hue.ApplyTo(new Color(1f, 0f, 0f));
		}
		#endregion Casts

		#region Operators
		#region -
		public static Hue operator -(Hue v1, float v2)
		{
			return v1.Value - v2;
		}

		public static Hue operator -(Hue v1, Hue v2)
		{
			return v1.Value - v2.Value;
		}

		public static Color operator -(Color color, Hue hue)
		{
			return (color.Hue - hue).ApplyTo(color);
		}

		public static Color operator -(Hue hue, Color color)
		{
			return (hue - color.Hue).ApplyTo(color);
		}
		#endregion -

		#region +
		public static Hue operator +(Hue v1, float v2)
		{
			return v1.Value + v2;
		}

		public static Hue operator +(Hue v1, Hue v2)
		{
			return v1.Value + v2.Value;
		}

		public static Color operator +(Color color, Hue hue)
		{
			return (color.Hue + hue).ApplyTo(color);
		}

		public static Color operator +(Hue hue, Color color)
		{
			return (hue + color.Hue).ApplyTo(color);
		}
		#endregion +

		public static bool operator ==(Hue v1, Hue v2)
		{
			float eps = v1.Epsilon;
			if (eps == EpsilonDefault || v2.Epsilon != EpsilonDefault && eps < v2.Epsilon)
				eps = v2.Epsilon;
			float d = Math.Abs(v2.Value - v1.Value);
			d %= 360;
			return d <= eps;
		}
		public static bool operator !=(Hue v1, Hue v2)
		{
			return !(v1 == v2);
		}

		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}
		#endregion Operators

		float _Value;

		public float Value
		{
			get { return _Value; }
			set { _Value = value % 360f; }
		}

		float _Epsilon;

		public float Epsilon
		{
			get { return _Epsilon; }
			set { _Epsilon = value; }
		}

		public const float EpsilonDefault = 2;

		public Color ApplyTo(Color color)
		{
			float h = this.Value;
			int hi = (int)(this.Value / 60) % 6;
			float s = color.Saturation;
			float v = color.Brightness;

			float f = h / 60 - hi;

			float q = v * (1 - s);
			float p = v * (1 - f * s);
			float t = v * (1 - (1 - f) * s);

			switch (hi)
			{
				case 0:
					return color.Set(v, t, p);
				case 1:
					return color.Set(q, v, p);
				case 2:
					return color.Set(p, v, t);
				case 3:
					return color.Set(p, q, v);
				case 4:
					return color.Set(t, p, v);
				case 5:
					return color.Set(v, p, q);
			}

			return color;
		}



		#region IComparable Members

		public int CompareTo(object obj)
		{
			if (obj is Hue)
				return this.CompareTo((Hue)obj);
			if (obj is Color)
				return this.CompareTo((Color)obj);
			if (obj is float)
				return this.CompareTo((float)obj);
			if (obj is double)
				return this.CompareTo((float)(double)obj);
			return -1;
		}

		#endregion

		#region IComparable<float> Members

		public int CompareTo(float other)
		{
			return this.Value.CompareTo(other);
		}

		#endregion

		#region IComparable<Hue> Members

		public int CompareTo(Hue other)
		{
			return this.CompareTo(other.Value);
		}

		#endregion

		#region IComparable<Color> Members

		public int CompareTo(Color other)
		{
			return this.CompareTo(other.Hue);
		}

		#endregion

		public override string ToString()
		{
			return this.Value.ToString() + "�";
		}
	}
}
