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

namespace zeroflag.Serialization.Descriptors
{
	public class DictionaryDescriptor<T1, T2> : Descriptor<System.Collections.Generic.IDictionary<T1, T2>>
	{
		public const string NameItem = "Item";

		protected override void DoParse()
		{
			if (this.Value != null)
			{

				IDictionary<T1, T2> value = this.GetValue();

				foreach (KeyValuePair<T1, T2> pair in value)
				{
					Helper helper = new Helper(pair.Key, pair.Value);
					Descriptor item = DoParse(helper, this);
					item.Name = NameItem;
				}
			}
		}

		//protected override object DoGenerate()
		//{
		//    if (this.Value == null)
		//        this.Value = this.DoCreateInstance();
		//    IDictionary<T1, T2> value = this.GetValue();

		//    if (value != null && this.Inner.Count > 0)
		//    {
		//        value.Clear();
		//        foreach (Descriptor sub in this.Inner)
		//        {
		//            if (sub.Name == NameItem)
		//            {
		//                Helper helper = (Helper)(/*sub.Value ?? */sub.Generate());
		//                //KeyValuePair<T1, T2> pair = (KeyValuePair<T1, T2>)sub.Generate();
		//                value.Add(helper.Key, helper.Value);
		//            }
		//        }
		//    }
		//    return this.Value;
		//}

		public override object GenerateLink()
		{
			//if (this.Value == null)
			//    this.Value = this.DoCreateInstance();
			IDictionary<T1, T2> value = this.GetValue();

			if (value != null && this.Inner.Count > 0)
			{
				value.Clear();
				foreach (Descriptor sub in this.Inner)
				{
					if (sub.Name == NameItem)
					{
						Helper helper = (Helper)(/*sub.Value ?? */sub.GenerateLink());
						//KeyValuePair<T1, T2> pair = (KeyValuePair<T1, T2>)sub.Generate();
						value.Add(helper.Key, helper.Value);
					}
				}
			}
			return this.Value;
		}
		public struct Helper
		{
			public Helper(T1 key, T2 value)
			{
				_Key = key;
				_Value = value;
			}

			T1 _Key;

			public T1 Key
			{
				get { return _Key; }
				set { _Key = value; }
			}

			T2 _Value;

			public T2 Value
			{
				get { return _Value; }
				set { _Value = value; }
			}
		}
	}
}
