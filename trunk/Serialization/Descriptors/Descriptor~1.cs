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
	public abstract class Descriptor<T> : Descriptor
	{
		//GetHandler<T> _Get;

		//public GetHandler<T> Get
		//{
		//    get { return _Get; }
		//    set { _Get = value; }
		//}

		//SetHandler<T> _Set;

		//public SetHandler<T> Set
		//{
		//    get { return _Set; }
		//    set { _Set = value; }
		//}

		//T _Value = default(T);
		//public T Value
		//{
		//    get
		//    {
		//        return this.Get != null ? this.Get() : _Value;
		//    }
		//    set
		//    {
		//        if (this.Set != null)
		//            this.Set(value);
		//        _Value = value;
		//    }
		//}
		public virtual T GetValue()
		{
			return (T)this.Value;
		}
		public virtual void SetValue(T value)
		{
			this.Value = (T)value;
		}

		public override int Id
		{
			get
			{
				if (this.Value != null)
					return this.Value.GetHashCode();
				else
					return base.Id;
			}
		}

		public override Type Type
		{
			get
			{
				return this.Value != null ? this.Value.GetType() : base.Type ?? typeof(T);
			}
			set
			{
				base.Type = value;
			}
		}

		public override Descriptor Parse(System.Reflection.PropertyInfo info)
		{
			//this.Get = delegate() { return (T)info.GetGetMethod().Invoke(this.Owner.GetValue(), null); };
			//this.Set = delegate(T value) { info.GetSetMethod().Invoke(this.Owner.GetValue(), new object[] { value }); };
			if (info.CanRead && info.CanWrite
				//				&& this.Owner != null 
				)
			{
				object value = null;
				if (this.Owner != null && this.Owner.Value != null)
					value = info.GetValue(this.Owner.Value, new object[] { });
				return this.Parse(info.Name, info.PropertyType, value);
			}
			else return this;
		}

		public override Descriptor Parse(string name, Type type, object value)
		{
			this.Value = (T)value;
			return this.Parse(name, type);
		}

		public override Descriptor Parse(string name, Type type, Descriptor owner, object value)
		{
			this.Value = (T)value;
			return this.Parse(name, type, owner);
		}
	}
}
