using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization
{
	public class ExceptionCollection : zeroflag.Collections.Collection<ExceptionTrace>
	{
		[System.ComponentModel.TypeConverter( typeof( System.ComponentModel.CollectionConverter ) )]
		public zeroflag.Collections.Collection<ExceptionTrace> Exceptions
		{
			get { return this; }
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach ( ExceptionTrace exc in this )
			{
				sb.Append( exc.ToString() ).AppendLine();
			}
			return sb.ToString();
		}
	}
}
