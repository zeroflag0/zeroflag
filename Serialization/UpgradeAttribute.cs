#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag >>at<< zeroflag >>dot<< de
//	
//	Copyright (C) 2006-2009  Thomas "zeroflag" Kraemer
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

namespace zeroflag.Serialization
{
	public class UpgradeAttribute : SerializationAttribute
	{
		string[] _Previous = new string[0];

		public string[] Previous
		{
			get { return _Previous; }
			set { _Previous = value; }
		}

		public UpgradeAttribute(params string[] from)
		{
			this.Previous = from;
		}

		public override void ApplyTo(zeroflag.Serialization.Descriptors.Descriptor desc)
		{
			Type type = desc.Type;
			List<string> previous = new List<string>(this.Previous);
			foreach (System.Reflection.PropertyInfo prop in desc.GetProperties(type))
			{
				if (previous.Contains(prop.Name))
				{
					desc.Name = prop.Name;
					break;
				}
			}
		}
	}
}
