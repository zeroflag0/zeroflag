using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization
{
	public class IgnoreAttribute : Attribute
	{
		public IgnoreAttribute(bool ignore)
		{
			this.Ignore = ignore;
		}

		bool _Ignore = false;

		public bool Ignore
		{
			get { return _Ignore; }
			set { _Ignore = value; }
		}

		public static implicit operator bool?(IgnoreAttribute attr)
		{
			if (attr != null)
				return attr.Ignore;
			else
				return null;
		}
	}
}
