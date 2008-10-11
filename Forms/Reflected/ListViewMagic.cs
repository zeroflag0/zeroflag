using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zeroflag.Forms.Reflected
{
	public partial class ListViewMagic : UserControl
	{
		public ListViewMagic()
		{
			InitializeComponent();
		}


		#region ItemType

		private Type _ItemType;

		/// <summary>
		/// The type of items displayed by this view.
		/// </summary>
		public Type ItemType
		{
			get { return _ItemType; }
			set
			{
				if ( _ItemType != value )
				{
					this.OnItemTypeChanged( _ItemType, _ItemType = value );
					this.SpecializedView = null;
					if ( this.SpecializedView != null )
						this.SpecializedView.Update();
				}
			}
		}

		#region ItemTypeChanged event
		public delegate void ItemTypeChangedHandler( object sender, Type oldvalue, Type newvalue );

		private event ItemTypeChangedHandler _ItemTypeChanged;
		/// <summary>
		/// Occurs when ItemType changes.
		/// </summary>
		public event ItemTypeChangedHandler ItemTypeChanged
		{
			add { this._ItemTypeChanged += value; }
			remove { this._ItemTypeChanged -= value; }
		}

		/// <summary>
		/// Raises the ItemTypeChanged event.
		/// </summary>
		protected virtual void OnItemTypeChanged( Type oldvalue, Type newvalue )
		{
			// if there are event subscribers...
			if ( this._ItemTypeChanged != null )
			{
				// call them...
				this._ItemTypeChanged( this, oldvalue, newvalue );
			}
		}
		#endregion ItemTypeChanged event
		#endregion ItemType


		#region Items

		private object _Items;

		/// <summary>
		/// The item-collection for this view. (must be derived from zeroflag.Collections.List)
		/// </summary>
		public object Items
		{
			get { return _Items; }
			set
			{
				if ( _Items != value )
				{
					if ( value != null )
					{
						if ( !typeof( ICollection<> ).IsAssignableFrom( value.GetType() ) )
						{
							throw new ArgumentException( "The collection passed to this ListView does not implement ICollection<> (" + value.GetType() + ")" );
						}
						else
						{
							Type itemtype = null;
							Type type = value.GetType();
							foreach ( var prop in type.GetProperties() )
							{
								var paras = prop.GetIndexParameters();
								if ( paras != null && paras.Length == 1 )
								{
									itemtype = paras[ 0 ].ParameterType;
								}
							}
							if ( itemtype == null )
								throw new ArgumentException( "Cannot find the itemtype on collection (" + value.GetType() + ")" );
							_Items = value;
							this.ItemType = itemtype;
						}
					}
					_Items = value;
				}
			}
		}

		#endregion Items



		#region SpecializedView
		private ListViewControlBase _SpecializedView;

		/// <summary>
		/// The specialized list-view for a specific type.
		/// </summary>
		public ListViewControlBase SpecializedView
		{
			get { return _SpecializedView ?? ( _SpecializedView = this.SpecializedViewCreate ); }
			protected set { _SpecializedView = value; }
		}

		/// <summary>
		/// Creates the default/initial value for SpecializedView.
		/// The specialized list-view for a specific type.
		/// </summary>
		protected virtual ListViewControlBase SpecializedViewCreate
		{
			get
			{
				if ( this.ItemType == null )
					return null;
				ListViewControlBase view = (ListViewControlBase)zeroflag.Reflection.TypeHelper.CreateInstance( typeof( ListView<> ), this.ItemType );

				try
				{
					this.SuspendLayout();
					this.Controls.Clear();
					this.Controls.Add( view );
					view.Dock = DockStyle.Fill;
				}
				finally
				{
					this.ResumeLayout();
				}

				return view;
			}
		}

		#endregion SpecializedView

	}
}
