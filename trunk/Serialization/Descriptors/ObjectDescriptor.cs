using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public class ObjectDescriptor : Descriptor<object>
	{
		public override Descriptor Parse()
		{
			if (this.Value == null)
				return this;

			Type type = this.Type;

			System.Reflection.PropertyInfo[] properties = type.GetProperties();

			foreach (System.Reflection.PropertyInfo property in properties)
			{
				Descriptor.DoParse(property, this);
			}

			return this;
		}
	}
}
