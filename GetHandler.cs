using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag
{
	public delegate R GetHandler<R>();
	public delegate R GetHandler<R, T1>( T1 p1 );
	public delegate R GetHandler<R, T1, T2>( T1 p1, T2 p2 );
	public delegate R GetHandler<R, T1, T2, T3>( T1 p1, T2 p2, T3 p3 );
}
