using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization
{
	public abstract class SerializationAttribute : Attribute
	{
		public abstract void ApplyTo(Descriptors.Descriptor desc);
	}
}
