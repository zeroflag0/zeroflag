using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public class StringDescriptor : Descriptor<string>
	{
		public override Descriptor Parse()
		{
			if (this.Value == null)
				return this;

			Type type = this.Type;

			return this;
		}
	}
}
