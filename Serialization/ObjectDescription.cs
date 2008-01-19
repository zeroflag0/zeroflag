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
