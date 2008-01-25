using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public abstract class Descriptor<T> : Descriptor
	{
		GetHandler<T> _Get;

		public GetHandler<T> Get
		{
			get { return _Get; }
			set { _Get = value; }
		}

		SetHandler<T> _Set;

		public SetHandler<T> Set
		{
			get { return _Set; }
			set { _Set = value; }
		}

		T _Value = default(T);
		public T Value
		{
			get
			{
				return this.Get != null ? this.Get() : _Value;
			}
			set
			{
				if (this.Set != null)
					this.Set(value);
				_Value = value;
			}
		}
		public override object GetValue()
		{
			return this.Value;
		}
		public override void SetValue(object value)
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
				object value = info.GetValue(this.Owner.GetValue(), new object[] { });
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
