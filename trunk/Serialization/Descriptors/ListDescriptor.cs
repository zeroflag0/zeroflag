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

		public override Descriptor Parse()
		{
			if (this.Value == null)
				return this;

			if (this.Value != null)
			{
				DoParse(this.GetValue().Count, this).Name = "Count";

				foreach (object value in this.GetValue())
				{
					Descriptor item =  DoParse(value, typeof(T), this);
					item.Name = "Items";
					//this.Inner.Add(item);
					//item.Set = delegate(object value) { this.Value
				}
			}

			return this;
		}

	}
}
