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
		}
	}
}