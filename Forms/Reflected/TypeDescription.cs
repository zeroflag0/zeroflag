using System;
using System.Collections.Generic;
using System.Reflection;

namespace zeroflag.Forms.Reflected
{
	[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
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
			//set
			//{
			//    if (_Properties != value)
			//    {
			//        _Properties = value;
			//    }
			//}
		}
		protected virtual Collections.Collection<PropertyDescription> PropertiesCreate
		{
			get
			{
				var props = new zeroflag.Collections.Collection<PropertyDescription>();

				props.ItemAdded += new zeroflag.Collections.List<PropertyDescription>.ItemAddedHandler(props_ItemAdded);
				props.ItemRemoved += new zeroflag.Collections.List<PropertyDescription>.ItemRemovedHandler(props_ItemRemoved);

				return props;
			}
		}

		void props_ItemAdded(PropertyDescription item)
		{
			if (item != null) item.Changed += new PropertyDescription.ChangedHandler(item_Changed);
		}

		void props_ItemRemoved(PropertyDescription item)
		{
			if (item != null) item.Changed -= new PropertyDescription.ChangedHandler(item_Changed);
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
