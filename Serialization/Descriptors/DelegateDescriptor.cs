using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public class DelegateDescriptor : Descriptor<Delegate>
	{
		protected override void DoParse()
		{
			if (this.Value == null)
				return;

			Type type = this.Type;

			Delegate dele = this.Value as Delegate;

		}
	}
}
