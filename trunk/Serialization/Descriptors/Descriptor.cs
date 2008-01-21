using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public abstract class Descriptor
	{
		public static Descriptor Parse(Type t)
		{
			return Parse(t, null);
		}

		public static Descriptor Parse(object value)
		{
			return Parse(value.GetType(), value);
		}

		public static Descriptor Parse(Type t, object value)
		{
		}



		public delegate T GetHandler<T>();
		public delegate void SetHandler<T>(T value);

		private Type _Type;

		public virtual Type Type
		{
			get { return _Type; }
			set { _Type = value; }
		}

		object _Owner;

		public virtual object Owner
		{
			get { return _Owner; }
			set { _Owner = value; }
		}

		public abstract object GetValue();
		public abstract void SetValue(object value);

		public void Parse(System.Reflection.PropertyInfo info, object owner)
		{
			this.Owner = owner;
			this.Parse(info);
		}

		public abstract void Parse(System.Reflection.PropertyInfo info);
	}
}
