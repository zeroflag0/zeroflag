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
	public class ListDescriptor<T> : Descriptor<System.Collections.Generic.IList<T>>
	{
		//public override Type Type
		//{
		//    get
		//    {
		//        return this.Value != null ? this.Value.GetType() : (base.Type == typeof(System.Collections.IEnumerable) ? typeof(System.Collections.Generic.IList<>) : base.Type);
		//    }
		//    set
		//    {
		//        base.Type = value;
		//    }
		//}

		//public override Descriptor Parse(System.Reflection.PropertyInfo info)
		//{
		//    //this.Get = delegate() { return (System.Collections.Generic.IList<T>)info.GetGetMethod().Invoke(this.Owner.GetValue(), null); };
		//    //this.Set = delegate(T value) { info.GetSetMethod().Invoke(this.Owner.GetValue(), new object[] { value }); };

		//    return this.Parse();
		//}
		public const string NameItem = null;

		protected override void DoParse()
		{
			if (this.Value != null)
			{
				CWL(this + " parsing " + this.Value);
				System.Collections.Generic.IList<T> value = this.GetValue();
				int max = value.Count;
				for (int i = 0; i < max; i++)
				{
					Descriptor item = DoParse(value[i], typeof(T), this);
					item.Name = NameItem;
				}
			}
		}

		//protected override object DoGenerate()
		//{
		//    if (this.Value == null)
		//        this.Value = this.DoCreateInstance();
		//    IList<T> value = this.GetValue();

		//    if (value != null && this.Inner.Count > 0)
		//    {
		//        value.Clear();
		//        foreach (Descriptor sub in this.Inner)
		//        {
		//            if (sub.Name == NameItem)
		//            {
		//                value.Add((T)sub.Generate());
		//            }
		//        }
		//    }
		//    return this.Value;
		//}

		public override object GenerateLink()
		{
			//return base.GenerateLink();
			IList<T> value = this.GetValue();

			if (value != null && this.Inner.Count > 0)
			{
				value.Clear();
				foreach (Descriptor sub in this.Inner)
				{
					//if (sub.Name == NameItem)
					//{
						value.Add((T)sub.GenerateLink());
					//}
				}
			}
			return this.Value;
		}

		//protected override void SetValue(object on, System.Reflection.PropertyInfo prop)
		//{
		//    this.Value = this.GenerateLink();
		//    IList<T> value = this.GetValue();

		//    if (value != null)
		//    {
		//        if (prop != null)
		//        {
		//            IList<T> onValue = null;
		//            if ((onValue = prop.GetValue(on, new object[0]) as IList<T>) == null)
		//                prop.SetValue(on, this.GenerateLink(), new object[] { });
		//            else
		//            {
		//                foreach (T item in value)
		//                {
		//                    onValue.Add(item);
		//                }
		//            }
		//        }
		//    }
		//}
	}
}
