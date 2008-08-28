using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace zeroflag.Forms.Reflected
{
	public partial class PropertyTreeView : TreeView<object>
	{
		public PropertyTreeView()
		{
			InitializeComponent();
		}

		protected override TreeView<object>.GetChildEnumeratorHandler GetChildEnumeratorCallbackCreate
		{
			get
			{
				return this.GetChildrenEnumerator;
			}
		}

		protected virtual IEnumerable<object> GetChildrenEnumerator( object item )
		{
			if ( item == null )
				yield break;
			if ( item is PropertyDescriptor )
			{
				item = ( (PropertyDescriptor)item ).Value ?? item;
			}

			Type type = item.GetType();
			if ( this.Converters.CanConvert<string>( item ) )
			{
				//yield return this.Converters.Generate<string>( item );
				yield break;
			}
			else
			{
				//if ( typeof( IEnumerable<object> ).IsAssignableFrom( type ) )
				//{
				//    foreach ( object child in (IEnumerable<object>)item )
				//        yield return child;
				//}
				//else 
				if ( typeof( System.Collections.IEnumerable ).IsAssignableFrom( type ) )
				{
					var property = type.GetProperty( "Count", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public );
					if ( property != null )
						yield return new PropertyDescriptor() { Property = property, Value = property.GetValue( item, null ), Parent = item };
					foreach ( object child in (System.Collections.IEnumerable)item )
						yield return child;
				}
				else
				{
					object child;
					foreach ( var property in
						type.GetProperties( System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public ) )
					{
						if ( property.CanRead && property.GetIndexParameters().Length <= 0 )
						{
							child = property.GetValue( item, null );

							yield return new PropertyDescriptor() { Property = property, Value = child, Parent = item };
						}
					}
				}
			}
		}

		public class PropertyDescriptor
		{
			System.Reflection.PropertyInfo _Property;

			public System.Reflection.PropertyInfo Property
			{
				get { return _Property; }
				set { _Property = value; }
			}

			object _Value;

			public object Value
			{
				get { return _Value; }
				set { _Value = value; }
			}

			object _Parent;

			public object Parent
			{
				get { return _Parent; }
				set { _Parent = value; }
			}

			public override string ToString()
			{
				return this.Property.Name + ": " + this.Value;
			}

			public override bool Equals( object obj )
			{
				if ( obj is PropertyDescriptor )
				{
					PropertyDescriptor other = obj as PropertyDescriptor;
					return this.Value.Equals( other.Value ) && this.Property.Equals( other.Property ) &&
						(
						( object.ReferenceEquals( null, this.Parent ) && object.ReferenceEquals( null, other.Parent ) ) ||
						( !object.ReferenceEquals( null, this.Parent ) && this.Parent.Equals( other.Parent ) )
						)
						;
				}
				else
					return this.Value.Equals( obj );
			}

			public override int GetHashCode()
			{
				return this.Value.GetHashCode();
			}
		}

		//public override void SynchronizeSelected()
		//{
		//    if ( this.SelectedNode != null )
		//    {
		//        object value = ( this.SelectedNode as TreeViewItem<object> ).Value;
		//        //if ( value is PropertyDescriptor )
		//        //    value = ( value as PropertyDescriptor ).Value;
		//        this.SelectedItem = value;
		//        this.SelectedNode.Expand();
		//    }
		//    else
		//    {
		//        this.SelectedItem = null;
		//    }
		//}

		#region Converters
		private zeroflag.Serialization.Converters.ConverterCollection _Converters;

		/// <summary>
		/// Value Converters.
		/// </summary>
		public zeroflag.Serialization.Converters.ConverterCollection Converters
		{
			get { return _Converters ?? ( _Converters = this.ConvertersCreate ); }
			//set { _Converters = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Converters.
		/// Value Converters.
		/// </summary>
		protected virtual zeroflag.Serialization.Converters.ConverterCollection ConvertersCreate
		{
			get { return new zeroflag.Serialization.Converters.ConverterCollection(); }
		}

		#endregion Converters



		#region ValueProperties
		private Dictionary<object, System.Reflection.PropertyInfo> _ValueProperties;

		/// <summary>
		/// The properties from which values were retreived.
		/// </summary>
		public Dictionary<object, System.Reflection.PropertyInfo> ValueProperties
		{
			get { return _ValueProperties ?? ( _ValueProperties = this.ValuePropertiesCreate ); }
			//set { _ValueProperties = value; }
		}

		/// <summary>
		/// Creates the default/initial value for ValueProperties.
		/// The properties from which values were retreived.
		/// </summary>
		protected virtual Dictionary<object, System.Reflection.PropertyInfo> ValuePropertiesCreate
		{
			get { return new Dictionary<object, System.Reflection.PropertyInfo>(); }
		}

		#endregion ValueProperties


		protected override TreeViewItem<object> ProvideViewOuter( object parent )
		{
			TreeViewItem<object> outer = null;
			if ( parent != null )
			{
				object value = parent;
				if ( parent is PropertyDescriptor )
					value = ( (PropertyDescriptor)parent ).Value;
				outer = new List<TreeViewItem<object>>( this.ItemPeers.Values ).Find( view => ( view.Value is PropertyDescriptor ) ? ( (PropertyDescriptor)view.Value ).Value == value : view.Value == value );
			}
			return outer;
			//return base.ProvideViewOuter( parent );
		}

		protected override TreeViewItem<object> ProvideViewItem( object item, TreeViewItem<object> outer )
		{
			TreeViewItem<object> view = null;
			if ( this.ItemPeers.ContainsKey( item ) )
				view = this.ItemPeers[ item ];
			if ( view == null && item is PropertyDescriptor )
			{
				object value = ( (PropertyDescriptor)item ).Value;
				view = new List<TreeViewItem<object>>( this.ItemPeers.Values ).Find( v => ( v.Value is PropertyDescriptor ) ? ( (PropertyDescriptor)v.Value ).Value == value : v.Value == value );

				if ( new List<object>( this.ItemPeers.Keys ).Find( k => ( k is PropertyDescriptor ) ? ( (PropertyDescriptor)k ).Value == value : k == value ) == null )
				{
					view = this.InitializeViewItem( this.CreateViewItem( item ), item, outer );
				}
			}
			if ( view == null )
			{
				// create peer...
				view = this.InitializeViewItem( this.CreateViewItem( item ), item, outer );
			}
			return view;
		}

		protected TreeViewItem<object> FindView( object value )
		{
			if ( value is PropertyDescriptor )
			{
				var desc = value as PropertyDescriptor;
				value = desc.Value;
				return new List<TreeViewItem<object>>( this.ItemPeers.Values ).Find( view => desc.Equals( view.Value ) );
			}
			else
			{
				return new List<TreeViewItem<object>>( this.ItemPeers.Values ).Find( view => ( view.Value is PropertyDescriptor ) ? ( (PropertyDescriptor)view.Value ).Equals( value ) : view.Value == value );
			}
		}

		protected override TreeViewItem<object> CreateViewItem( object item )
		{
			return new PropertyTreeViewItem() { Value = item };
		}

		//public override string Decorate( object node )
		//{
		//    if ( node == null || node.Value == null )
		//        return "<null>";

		//    string name = "";
		//    object value = node.Value;
		//    if ( node is PropertyTreeViewItem )
		//    {
		//        var desc = ( (PropertyTreeViewItem)node ).Descriptor;
		//        if ( desc != null )
		//        {
		//            value = desc.Value;
		//            name = desc.Property.Name + ": ";
		//        }
		//    }


		//    //if ( node.Value != null && this.ValueProperties.ContainsKey( node.Value ) )
		//    //{
		//    //    name = this.ValueProperties[ node.Value ].Name + ": ";
		//    //}
		//    //else if ( ( node.Parent as TreeViewItem<object> ) != null )
		//    //{
		//    //var parent = node.Parent as TreeViewItem<object>;
		//    //if ( this.ValueProperties.ContainsKey( parent.Value ) )
		//    //{
		//    //    this.ValueProperties[parent.Value].
		//    //}
		//    //    if ( parent.Value != null )
		//    //    {
		//    //        var props = new List<System.Reflection.PropertyInfo>( parent.Value.GetType().GetProperties() );
		//    //        var prop = props.Find( p => p.PropertyType.IsAssignableFrom( node.Value.GetType() ) &&
		//    //            p.GetValue( parent.Value, null ) == node.Value );
		//    //        if ( prop != null )
		//    //            name = prop + ": ";
		//    //    }
		//    //}
		//    if ( this.Converters.CanConvert<string>( value ) )
		//        return name + this.Converters.Generate<string>( value );

		//    if ( value == null )
		//        return name + "<null>";

		//    return name + this.FindDecorator( value.GetType() )( node );
		//}

		protected override Dictionary<Type, ValueDecorationHandler<object>> ValueDecoratorsCreate
		{
			get
			{
				var decs = base.ValueDecoratorsCreate;
				decs.Add( typeof( System.Collections.IEnumerable ), val =>
					{
						string text = "";
						var prop = val.GetType().GetProperty( "Count" ) ?? val.GetType().GetProperty( "Length" );
						if ( prop != null )
						{
							var value = prop.GetValue( val, null );
							if ( value != null )
							{
								text = this.Decorate( value ) + ": ";
							}
						}
						return text + this.DefaultValueDecorator( val );
					} );
				return decs;
			}
		}
	}
}
