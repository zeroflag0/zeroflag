using System;
using System.Collections.Generic;

using System.Text;

namespace zeroflag.Forms.Reflected
{
	public enum FilterVisibility
	{
		Enabled,
		Disabled,
		HiddenCompletely,
	}
	public delegate FilterVisibility FilterTypeHandler<T>( T item, Type type );
	public delegate FilterVisibility FilterTypeHandler( Type type );
}
