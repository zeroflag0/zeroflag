using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	class DictionaryDescriptor<T1, T2> : Descriptor<System.Collections.Generic.Dictionary<T1, T2>>
	{
		protected override void DoParse()
		{
			throw new NotImplementedException();
		}
	}
}
