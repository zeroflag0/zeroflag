using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Forms.Reflected
{
	[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class PropertyDescription
	{
		public PropertyDescription(System.Reflection.PropertyInfo info)
		{
			this.PropertyInfo = info;
		}

		#region PropertyInfo

		private System.Reflection.PropertyInfo _PropertyInfo = default(System.Reflection.PropertyInfo);

		public System.Reflection.PropertyInfo PropertyInfo
		{
			get { return _PropertyInfo; }
			set
			{
				if (_PropertyInfo != value)
				{
					this.Describe(_PropertyInfo = value);
					this.OnChanged(this);
				}
			}
		}
		#endregion PropertyInfo

		#region Name

		private string _Name = default(string);
		[System.ComponentModel.Localizable(true)]
		public string Name
		{
			get { return _Name; }
			set
			{
				if (_Name != value)
				{
					_Name = value;
					this.OnChanged(this);
				}
			}
		}
		#endregion Name

		#region Visible

		private bool _Visible = true;

		public bool Visible
		{
			get { return _Visible; }
			set
			{
				if (_Visible != value)
				{
					_Visible = value;
					this.OnChanged(this);
				}
			}
		}

		#endregion Visible

		#region Type

		private Type _Type = default(Type);

		public Type Type
		{
			get { return _Type; }
			set
			{
				if (_Type != value)
				{
					_Type = value;
					this.OnChanged(this);
				}
			}
		}

		#endregion Type

		#region event Changed
		public delegate void ChangedHandler(PropertyDescription property);

		private event ChangedHandler _Changed;
		/// <summary>
		/// When the property's description changes
		/// </summary>
		public event ChangedHandler Changed
		{
			add { this._Changed += value; }
			remove { this._Changed -= value; }
		}
		/// <summary>
		/// Call to raise the Changed event:
		/// When the property's description changes
		/// </summary>
		protected virtual void OnChanged(PropertyDescription property)
		{
			// if there are event subscribers...
			if (this._Changed != null)
			{
				// call them...
				this._Changed(property);
			}
		}
		#endregion event Changed

		#region Describe
		public PropertyDescription Describe(System.Reflection.PropertyInfo info)
		{
			this.PropertyInfo = info;

			this.Name = info.Name;
			this.Type = info.PropertyType;
			this.Visible = true;
			object[] atts = info.GetCustomAttributes(typeof(System.ComponentModel.BrowsableAttribute), true);
			foreach (object o in atts)
			{
				System.ComponentModel.BrowsableAttribute attr = o as System.ComponentModel.BrowsableAttribute;
				if (attr != null)
				{
					this.Visible = attr.Browsable;
				}
			}

			return this;
		}
		#endregion

		public override string ToString()
		{
			return this.GetType().Name + "[" + this.Name + "," + this.Type + "]";
		}
	}
}
