using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public class ListDescriptor : Descriptor<System.Collections.IList>, IListDescriptor
	{
		#region ItemType

		private Type _ItemType = typeof( object );

		/// <summary>
		/// The most likely type for the items in this collection...
		/// </summary>
		public Type ItemType
		{
			get { return _ItemType; }
			set
			{
				if ( _ItemType != value )
				{
					_ItemType = value;
				}
			}
		}

		#endregion ItemType

		public override void Parse()
		{
			{
				CWL( this + " parsing " + ( this.Value ?? this.Type ?? (object)"<null>" ) );
				var collection = this.GetValue();
				if ( collection != null )
				{
					#region identify most likely item type...
					Type type = collection.GetType();
					List<System.Reflection.PropertyInfo> indexers = new List<System.Reflection.PropertyInfo>();
					foreach ( var prop in type.GetProperties() )
					{
						if ( prop.GetIndexParameters().Length == 0 )
							// don't care about normal properties...
							continue;
						if ( prop.PropertyType == typeof( object ) )
							// don't care about indexers that only return objects...
							continue;
						if ( this.ItemType == null || this.ItemType == typeof( object ) || this.ItemType.IsAssignableFrom( prop.PropertyType ) )
							// the indexer's type is a better match than what we already have?
							this.ItemType = prop.PropertyType;
					}
					foreach ( object value in collection )
					{
						if ( value == null )
							// don't care about nulls... what are they doing here anyway?
							continue;
						if ( value.GetType() == typeof( object ) )
							// don't care about objects... wtf?
							continue;
						if ( this.ItemType == null || this.ItemType == typeof( object ) || this.ItemType.IsAssignableFrom( value.GetType() ) )
							// the item's type is a better match than what we already have?
							this.ItemType = value.GetType();
					}
					#endregion identify most likely item type...

					foreach ( object value in collection )
					{
						Descriptor item = this.Context.Parse( null, ItemType, value, this.Value, null );
						//item.Name = NameItem;
						if ( !this.Inner.Contains( item ) )
							this.Inner.Add( item );
					}
				}
			}

		}

		public override object GenerateLink()
		{
			//return base.GenerateLink();
			var value = this.GetValue();

			if ( value != null && this.Inner.Count > 0 )
			{
				value.Clear();
				foreach ( Descriptor sub in this.Inner )
				{
					//if (sub.Name == NameItem)
					//{
					object item = sub.GenerateLink();
					if ( item != null )
						value.Add( item );
					//}
				}
			}
			return this.Value;
		}

		public override bool NeedsWriteAccess
		{
			get
			{
				return false;
			}
		}

	}
}
