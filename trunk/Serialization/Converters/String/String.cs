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

namespace zeroflag.Serialization.Converters.String
{
	public class String : Converter<System.String>
	{
#if REPLACE_MAP
		Dictionary<string, string> _ReplaceMap;
		protected Dictionary<string, string> ReplaceMap
		{
			get { return _ReplaceMap ?? (_ReplaceMap = CreateReplaceMap); }
		}
		protected virtual Dictionary<string, string> CreateReplaceMap
		{
			get
			{
				Dictionary<string, string> map =new Dictionary<string, string>();
				map.Add("\\", "\\\\");
				map.Add("\n", "\\n");
				map.Add("\r", "\\r");
				map.Add("\t", "\\t");
				map.Add("\b", "\\b");
				map.Add("\0", "\\0");
				return map;
			}
		}
#endif

		public override System.String ___Parse(Type type, string value)
		{
#if REPLACE_MAP
		foreach (string key in this.ReplaceMap.Keys)
				value = value.Replace(this.ReplaceMap[key], key);
#endif
			return value;
		}
#if REPLACE_MAP
		public override string _Generate(string value)
		{
			value = base._Generate(value);
			foreach (string key in this.ReplaceMap.Keys)
				value = value.Replace(key, this.ReplaceMap[key]);
			return value;
		}
#endif
	}
}
