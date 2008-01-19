using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.TypeDescriptors
{
	public class Default
	{
		#region Type
		Type _Type = null;

		public Type Type
		{
			get { return _Type; }
			set { _Type = value; }
		}
		#endregion

		#region Value
		object _Value = null;

		public object Value
		{
			get { return _Value ?? this.ValueGetter(); }
			set { _Value = value; }
		}

		public delegate object GetValueHandler();

		GetValueHandler _ValueGetter = null;

		public GetValueHandler ValueGetter
		{
			get { return _ValueGetter; }
			set { _ValueGetter = value; }
		}
		#endregion
	}
}
