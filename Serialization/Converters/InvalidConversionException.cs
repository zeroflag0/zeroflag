using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Converters
{
	public class InvalidConversionException : Exception
	{
		public InvalidConversionException(Type sourceType, Type targetType, object value, Exception inner)
			: base("Cannot convert from '" + sourceType + "' to '" + targetType + "' for value '" + (value ?? "<null>") + "' (" + (inner ?? new Exception()).Message + ")", inner)
		{
			this.SourceType = sourceType;
			this.TargetType = targetType;
		}
		public InvalidConversionException(Type sourceType, Type targetType, object value)
			: base("Cannot convert from '" + sourceType + "' to '" + targetType + "' for value '" + (value ?? "<null>") + "'")
		{
			this.SourceType = sourceType;
			this.TargetType = targetType;
		}

		Type _SourceType;

		public Type SourceType
		{
			get { return _SourceType; }
			set { _SourceType = value; }
		}

		Type _TargetType;

		public Type TargetType
		{
			get { return _TargetType; }
			set { _TargetType = value; }
		}
	}
}
