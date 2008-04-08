using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization
{
	public class ExceptionCollection : zeroflag.Collections.Collection<ExceptionTrace>
	{
		[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.CollectionConverter))]
		public zeroflag.Collections.Collection<ExceptionTrace> Exceptions
		{
			get { return this; }
		}
	}
}
