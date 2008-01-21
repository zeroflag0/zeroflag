using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public class Default : Descriptor<object>
	{
		public override void Parse(System.Reflection.PropertyInfo info)
		{
			this.Get = delegate() { return info.GetGetMethod().Invoke(this.Owner, null); };
		}
	}
}
