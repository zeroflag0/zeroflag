using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public class Default : Descriptor<object>
	{
		public override Descriptor Parse()
		{
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
