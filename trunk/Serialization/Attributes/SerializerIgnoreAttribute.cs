using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization
{
	public class SerializerIgnoreAttribute : zeroflag.Serialization.Attributes.Attribute
	{
		public SerializerIgnoreAttribute()
			: this( true )
		{
		}
		public SerializerIgnoreAttribute( bool ignore )
		{
			this.Ignore = ignore;
		}

		bool _Ignore = true;

		public bool Ignore
		{
			get { return _Ignore; }
			set { _Ignore = value; }
		}

		public static implicit operator bool?( SerializerIgnoreAttribute attr )
		{
			if ( attr != null )
				return attr.Ignore;
			else
				return null;
		}
	}
}
