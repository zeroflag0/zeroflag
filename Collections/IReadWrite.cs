using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Collections
{

	public interface IReadWrite<T>
	{
		T Read();
		void Write( T value );
		bool IsEmpty { get; }
	}
}
