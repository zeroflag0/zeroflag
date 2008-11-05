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
	public partial class ListViewMagic : UserControl, IListView
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
						Type type = value.GetType();
						if ( type.IsGenericType )
						{
							bool isCollection = false;
							Type generic = type.GetGenericTypeDefinition();
							foreach ( var intf in generic.GetInterfaces() )
							{
								if ( intf.Name.Contains( "ICollection" ) && intf.IsGenericType )
									isCollection = true;
							}
							if ( !isCollection )
							{
								throw new ArgumentException( "The collection passed to this ListView does not implement ICollection<> (" + value.GetType() + ")" );
							}
							else
							{
								Type itemtype = null;
								foreach ( var prop in type.GetProperties() )
								{
									var paras = prop.GetIndexParameters();
									if ( paras != null && paras.Length == 1 )
									{
										itemtype = prop.PropertyType;
									}
								}
								if ( itemtype == null )
									throw new ArgumentException( "Cannot find the itemtype on collection (" + value.GetType() + ")" );
								_Items = value;
								this.ItemType = itemtype;
							}
						}
						else if ( type.IsArray )
						{
							Type itemtype = type.GetElementType();
							if ( itemtype == null )
								throw new ArgumentException( "Cannot find the itemtype on collection (" + value.GetType() + ")" );
							Type listtype = typeof( zeroflag.Collections.List<> ).Specialize( itemtype );
							object list = listtype.CreateInstance();
							var addrange = listtype.GetMethod( "AddRange", new Type[] { type } );
							addrange.Invoke( list, new object[] { value } );
							_Items = list;
							this.ItemType = itemtype;
						}
						else
						{
							throw new ArgumentException( "Cannot interpret as any suitable collection (" + value.GetType() + ")" );
						}
					}
					//_Items = value;
					//if ( this.SpecializedView != null )
					//    ( (IListView)this.SpecializedView ).Items = (System.Collections.ICollection)this.Items;
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
				ListViewControlBase view = _SpecializedView = (ListViewControlBase)zeroflag.Reflection.TypeHelper.CreateInstance( typeof( ListView<> ), this.ItemType );
				var list = ( (IListView)this.SpecializedView );
				list.ItemAdded += new Action<object>( SpecializedViewItemAdded );
				list.ItemRemoved += new Action<object>( SpecializedViewItemRemoved );
				list.ItemSelected += new Action<object>( SpecializedViewItemSelected );
				list.ItemDeselected += new Action<object>( SpecializedViewItemDeselected );
				list.Sorting = this.Sorting;
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
				( (IListView)this.SpecializedView ).Items = (System.Collections.ICollection)this.Items;

				return view;
			}
		}

		void SpecializedViewItemDeselected( object obj )
		{
			if ( this._ItemDeselected != null )
				this._ItemDeselected( obj );
		}

		void SpecializedViewItemSelected( object obj )
		{
			if ( this._ItemSelected != null )
				this._ItemSelected( obj );
		}

		void SpecializedViewItemRemoved( object obj )
		{
			if ( this._ItemRemoved != null )
				this._ItemRemoved( obj );
		}

		void SpecializedViewItemAdded( object obj )
		{
			if ( this._ItemAdded != null )
				this._ItemAdded( obj );
		}

		#endregion SpecializedView


		public void Synchronize()
		{
			( (IListView)this.SpecializedView ).Synchronize();
		}

		#region IListView Members

		//public ListView Control
		//{
		//    get { return ( (IListView)this.SpecializedView ).Control; }
		//}

		event Action<object> _ItemAdded;

		public event Action<object> ItemAdded
		{
			add { _ItemAdded += value; }
			remove { _ItemAdded -= value; }
		}

		event Action<object> _ItemRemoved;

		public event Action<object> ItemRemoved
		{
			add { _ItemRemoved += value; }
			remove { _ItemRemoved -= value; }
		}

		event Action<object> _ItemDeselected;

		public event Action<object> ItemDeselected
		{
			add { _ItemDeselected += value; }
			remove { _ItemDeselected -= value; }
		}

		event Action<object> _ItemSelected;

		public event Action<object> ItemSelected
		{
			add { _ItemSelected += value; }
			remove { _ItemSelected -= value; }
		}

		public System.Collections.ICollection SelectedItems
		{
			get { return ( (IListView)this.SpecializedView ).SelectedItems; }
		}

		//public TypeDescription TypeDescription
		//{
		//    get
		//    {
		//        return ( (IListView)this.SpecializedView ).TypeDescription;
		//    }
		//    set
		//    {
		//        ( (IListView)this.SpecializedView ).TypeDescription = value;
		//    }
		//}

		#endregion

		#region IListView Members


		System.Collections.ICollection IListView.Items
		{
			get
			{
				return this.Items as System.Collections.ICollection;
			}
			set
			{
				this.Items = value;
			}
		}

		#endregion

		#region IListView Members

		SortOrder _Sorting;
		public SortOrder Sorting
		{
			get
			{
				try
				{
					return _Sorting = ( (IListView)this.SpecializedView ).Sorting;
				}
				catch
				{
					return _Sorting;
				}
			}
			set
			{
				_Sorting = value;
				try
				{
					( (IListView)this.SpecializedView ).Sorting = _Sorting;
				}
				catch { }
			}
		}

		#endregion
	}
}
