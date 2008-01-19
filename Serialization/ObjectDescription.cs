#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2007  Thomas "zeroflag" Kraemer
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
using System.Reflection;

namespace zeroflag.Serialization
{
	public class ObjectDescription
	{
		private List<ObjectDescription> m_Properties = new List<ObjectDescription>();

		public List<ObjectDescription> Properties
		{
			get { return m_Properties; }
		}

		public ObjectDescription(object value)
		{
			this.Value = value;
			this.Parse(value);
		}
		public ObjectDescription(Type type)
		{
			this.Type = type;
			this.Parse(type);
		}

		public ObjectDescription(object value, string name)
			: this(value)
		{
			this.Name = name;
		}
		public ObjectDescription(Type type, string name)
			: this(type)
		{
			this.Name = name;
		}
		public ObjectDescription(string name)
		{
			this.Name = name;
		}

		string m_Name = null;

		public string Name
		{
			get { return m_Name; }
			set { m_Name = value; }
		}

		#region Value

		private object m_Value = default(object);

		public object Value
		{
			get { return m_Value; }
			set
			{
				if (m_Value != value)
				{
					m_Value = value;
				}
			}
		}

		#endregion Value

		public Type Type
		{
			get
			{
				if (this.Value != null)
					return this.Value.GetType();
				else
					return null;
			}
			set
			{
				this.Value = TypeHelper.CreateInstance(value);
			}
		}

		public virtual void Parse(object value)
		{
			this.Properties.Clear();

			Type type = value.GetType();
			PropertyInfo[] props = type.GetProperties();

			foreach (PropertyInfo prop in props)
			{
				//TODO: check attributes...
				if (prop.GetGetMethod() != null && prop.GetSetMethod() != null)
				{
					ObjectDescription desc = new ObjectDescription(prop.PropertyType, prop.Name);
					this.Properties.Add(desc);
					if (prop.GetIndexParameters().Length == 0)
						desc.Parse(prop.GetValue(value, null));
				}
			}
		}

		public virtual void Parse(Type type)
		{
			//PropertyInfo[] props = type.GetProperties();

			//foreach (PropertyInfo prop in props)
			//{
			//    //TODO: check attributes...
			//    ObjectDescription desc = new ObjectDescription(prop.PropertyType, prop.Name);
			//    this.Properties.Add(desc);
			//    if (prop.GetIndexParameters().Length == 0)
			//        desc.Parse(prop.GetValue(value, null));
			//}
		}
	}
}
