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
		public virtual T GetValue()
		{
			return (T)this.Value;
		}
		public virtual void SetValue(T value)
		{
			this.Value = (T)value;
		}

		//static int count = 0;
		public override int? Id
		{
			get
			{
				if (this.Value != null && !this.Type.IsValueType)
					//	return base.Id = count++;
					return base.Id ?? this.Value.GetHashCode();
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
			if (info.CanRead && info.CanWrite)
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

		protected T Default
		{
			get { return default(T); }
		}
	}
}
