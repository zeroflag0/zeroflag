using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.StringConverters
{
	public class Int32 : Base<int>
	{
		public override string Write(int value)
		{
			return value.ToString();
		}

		public override int Parse(string text)
		{
			return int.Parse(text);
		}
	}
}
