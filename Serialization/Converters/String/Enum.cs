using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters.String
{
	public class Enum : Converter<System.Enum>
	{
		public override System.Enum ___Parse( Type type, string value )
		{
			try
			{
				return (System.Enum)System.Enum.Parse( type, value, true );
			}
			catch ( ArgumentException )
			{
#if !SILVERLIGHT
				foreach ( string val in System.Enum.GetNames( type ) )
					if ( val.ToLower() == value.ToLower() )
						return (System.Enum)System.Enum.Parse( type, val );
#endif
				throw;
			}
		}
	}
}
