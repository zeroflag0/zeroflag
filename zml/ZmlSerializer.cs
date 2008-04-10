using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Zml
{
	/// <summary>
	/// This class is only provided for backwards compatibility. if possible, upgrade to zeroflag.Serialization.ZmlSerializer.
	/// </summary>
	public class ZmlSerializer : zeroflag.Serialization.ZmlSerializer
	{
		public ZmlSerializer()
		{
		}

		public ZmlSerializer(string fileName)
			: base(fileName)
		{
		}

		public ZmlSerializer(ZmlSerializer parent)
			: base(parent)
		{
		}
	}
}
