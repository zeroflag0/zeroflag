using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag
{
	public static class Extensions
	{
		public static T Modify<T>( this T me, Action<T> action )
		{
			if ( action != null )
				action( (T)me );
			return me;
		}
	}
}
