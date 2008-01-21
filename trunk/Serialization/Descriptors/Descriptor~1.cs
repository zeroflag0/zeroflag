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

		public T Value
		{
			get
			{
				return this.Get != null ? this.Get() : default(T);
			}
			set
			{
				if (this.Set != null)
					this.Set(value);
			}
		}

		public override Type Type
		{
			get
			{
				return this.Value != null ? this.Value.GetType() : base.Type;
			}
			set
			{
				base.Type = value;
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
	}
}
