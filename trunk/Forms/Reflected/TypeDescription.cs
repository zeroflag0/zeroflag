using System;
using System.Collections.Generic;
using System.Reflection;

namespace zeroflag.Forms.Reflected
{
	[Serializable]
	//[System.ComponentModel.TypeConverter( typeof( System.ComponentModel.ExpandableObjectConverter ) )]
	[System.ComponentModel.Editor( typeof( TypeDescriptionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
	public class TypeDescription
	{
		#region Type

		private Type _Type = default(Type);

		public Type Type
		{
			get { return _Type; }
			set
			{
				if (_Type != value)
				{
					this.Describe(_Type = value);
				}
			}
		}

		#endregion Type

		#region Properties

		private Collections.Collection<PropertyDescription> _Properties;

		public Collections.Collection<PropertyDescription> Properties
		{
			get { return _Properties ?? (_Properties = this.PropertiesCreate); }
			set
			{
				if ( _Properties != value )
				{
					if ( _Properties != null )
					{
						_Properties.ItemAdded -= new zeroflag.Collections.List<PropertyDescription>.ItemAddedHandler( props_ItemAdded );
						_Properties.ItemRemoved -= new zeroflag.Collections.List<PropertyDescription>.ItemRemovedHandler( props_ItemRemoved );
						_Properties.ItemChanged -= new zeroflag.Collections.List<PropertyDescription>.ItemChangedHandler( _Properties_ItemChanged );
					}

					_Properties = value;

					if ( _Properties != null )
					{
						_Properties.ItemAdded += new zeroflag.Collections.List<PropertyDescription>.ItemAddedHandler( props_ItemAdded );
						_Properties.ItemRemoved += new zeroflag.Collections.List<PropertyDescription>.ItemRemovedHandler( props_ItemRemoved );
						_Properties.ItemChanged += new zeroflag.Collections.List<PropertyDescription>.ItemChangedHandler( _Properties_ItemChanged );
						foreach ( var prop in _Properties )
							props_ItemAdded( prop );
					}
				}
			}
		}

		protected virtual Collections.Collection<PropertyDescription> PropertiesCreate
		{
			get
			{
				return new zeroflag.Collections.Collection<PropertyDescription>();
			}
		}

		void _Properties_ItemChanged( object sender, PropertyDescription oldvalue, PropertyDescription newvalue )
		{
			props_ItemRemoved( oldvalue );
			props_ItemAdded( newvalue );
		}

		void props_ItemAdded(PropertyDescription item)
		{
			if ( item != null )
			{
				item.Owner = this;
				item.Changed += new PropertyDescription.ChangedHandler( item_Changed );
			}
		}

		void props_ItemRemoved(PropertyDescription item)
		{
			if ( item != null )
			{
				item.Owner = null;
				item.Changed -= new PropertyDescription.ChangedHandler( item_Changed );
			}
		}

		void item_Changed(PropertyDescription property)
		{
			this.OnChanged(this);
		}

		#endregion Properties

		#region event Changed
		public delegate void ChangedHandler(TypeDescription type);

		private event ChangedHandler _Changed;
		/// <summary>
		/// When the type description was modified.
		/// </summary>
		public event ChangedHandler Changed
		{
			add { this._Changed += value; }
			remove { this._Changed -= value; }
		}
		/// <summary>
		/// Call to raise the Changed event:
		/// When the type description was modified.
		/// </summary>
		protected virtual void OnChanged(TypeDescription type)
		{
			// if there are event subscribers...
			if (this._Changed != null)
			{
				// call them...
				this._Changed(type);
			}
		}
		#endregion event Changed

		public TypeDescription()
		{
		}

		public TypeDescription(Type type)
		{
			this.Describe(type);
		}

		public TypeDescription(object value)
		{
			this.Describe(value.GetType());
		}

		public TypeDescription Describe(Type type)
		{
			this.Type = type;

			this.Properties.Clear();
			var properties = type.GetProperties();
			foreach (var prop in properties)
			{
				if (prop.CanRead)
				{
					this.Properties.Add(new PropertyDescription(prop));
				}
			}

			return this;
		}

	}
}
