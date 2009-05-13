using System;
namespace System
{
	public class SerializableAttribute : Attribute
	{
	}

	namespace ComponentModel
	{
		public class BrowsableAttribute : Attribute
		{
			public BrowsableAttribute( bool value )
			{
			}
		}
		public class ListBindableAttribute : Attribute
		{
			public ListBindableAttribute( bool value )
			{
			}
		}

		public class CollectionConverter
		{
		}
		public class ExpandableObjectConverter
		{
			public virtual bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
			{
				return false;
			}

			public virtual object ConvertTo( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType )
			{
				return null;
			}

			public virtual bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
			{
				return false;
			}

			public virtual object ConvertFrom( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value )
			{
				return null;
			}
		}
	}
}