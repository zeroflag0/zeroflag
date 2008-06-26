using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace zeroflag.Forms.Reflected
{
	[Serializable]
	public partial class ListView<T> : UserControl
	{
		public ListView()
		{
			InitializeComponent();
			//this.Synchronize();

			//new zeroflag.Forms.DebugForm(AppDomain.CurrentDomain.GetAssemblies());
			this.Control.SelectedIndexChanged += new EventHandler( ListViewControl_SelectedIndexChanged );
		}

		void ListViewControl_SelectedIndexChanged( object sender, EventArgs e )
		{
			foreach ( ListViewItem<ListView<T>, T> view in this.Control.SelectedItems )
			{
				T item = this.ItemSync[ view ];
				if ( !this.SelectedItems.Contains( item ) )
					this.SelectedItems.Add( item );
			}
			List<T> selected = new List<T>( this.SelectedItems );
			foreach ( T item in selected )
			{
				var view = this.ItemSync[ item ];
				if ( !this.Control.SelectedItems.Contains( view ) )
					this.SelectedItems.Remove( item );
			}
			//this.SelectedItemSync.Synchronize();
		}

		#region TypeDescription

		private TypeDescription _TypeDescription = null;
		[Browsable( true )]
		[System.ComponentModel.Editor( typeof( TypeDescriptionEditor ), typeof( System.Drawing.Design.UITypeEditor ) )]
		public TypeDescription TypeDescription
		{
			get { return _TypeDescription ?? ( this.TypeDescription = this.ItemDescriptionCreate ); }
			set
			{
				if ( value != _TypeDescription )
				{
					if ( _TypeDescription != null )
						_TypeDescription.Changed -= new TypeDescription.ChangedHandler( TypeDescriptionChanged );

					_TypeDescription = value;

					if ( _TypeDescription != null )
						_TypeDescription.Changed += new TypeDescription.ChangedHandler( TypeDescriptionChanged );
				}
			}
		}
		protected virtual TypeDescription ItemDescriptionCreate
		{
			get
			{
				return new TypeDescription( typeof( T ) );
			}
		}

		void TypeDescriptionChanged( TypeDescription type )
		{
			this.Synchronize();
		}

		#endregion ItemDescription

		#region Items

		zeroflag.Collections.Collection<T> _Items;

		[MergableProperty( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		//[Editor(typeof(System.Windows.Forms.Design.ListViewItemCollectionEditor)
		//"System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
		//[Editor(typeof(zeroflag.Collections.Collection<>), typeof(System.Drawing.Design.UITypeEditor))]
		[Editor( "System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof( System.Drawing.Design.UITypeEditor ) )]
		[Browsable( true )]
		public zeroflag.Collections.Collection<T> Items
		{
			get { return _Items ?? ( this.Items = this.ItemsCreate ); }
			set
			{
				if ( value != this._Items )
				{
					if ( this._Items != null )
					{
						this._Items.ItemAdded -= __ItemAdded;
						this._Items.ItemRemoved -= __ItemRemoved;
					}

					this._Items = value;

					this._Control.Items.Clear();
					_ItemSync = null;
					_SelectedItems = null;
					_SelectedItemSync = null;

					if ( this._Items != null )
					{
						this._Items.ItemAdded += __ItemAdded;
						this._Items.ItemRemoved += __ItemRemoved;
					}
					this.Synchronize();
				}
			}
		}

		protected virtual zeroflag.Collections.Collection<T> ItemsCreate
		{
			get
			{
				zeroflag.Collections.Collection<T> items = new zeroflag.Collections.Collection<T>();

				return items;
			}
		}

		void __ItemRemoved( T item )
		{
			this.Synchronize();
			this.OnItemRemoved( item );
		}

		void __ItemAdded( T item )
		{
			this.Synchronize();
			this.OnItemAdded( item );
		}

		#region event ItemAdded
		private event Action<T> _ItemAdded;
		/// <summary>
		/// Item added
		/// </summary>
		public virtual event Action<T> ItemAdded
		{
			add { this._ItemAdded += value; }
			remove { this._ItemAdded -= value; }
		}
		/// <summary>
		/// Call to raise the ItemAdded event:
		/// Item added
		/// </summary>
		protected virtual void OnItemAdded( T item )
		{
			// if there are event subscribers...
			if ( this._ItemAdded != null )
			{
				// call them...
				this._ItemAdded( item );
			}
		}
		#endregion event ItemAdded

		#region event ItemRemoved
		private event Action<T> _ItemRemoved;
		/// <summary>
		/// Item removed.
		/// </summary>
		public virtual event Action<T> ItemRemoved
		{
			add { this._ItemRemoved += value; }
			remove { this._ItemRemoved -= value; }
		}
		/// <summary>
		/// Call to raise the ItemRemoved event:
		/// Item removed.
		/// </summary>
		protected virtual void OnItemRemoved( T item )
		{
			// if there are event subscribers...
			if ( this._ItemRemoved != null )
			{
				// call them...
				this._ItemRemoved( item );
			}
		}
		#endregion event ItemRemoved

		#region ItemChanged event
		//public delegate void ItemChangedHandler(object sender, T oldvalue, T newvalue);

		//private event ItemChangedHandler _ItemChanged;
		///// <summary>
		///// Occurs when any item changes. Also called when a item is added or removed.
		///// </summary>
		//public virtual event ItemChangedHandler ItemChanged
		//{
		//    add { this._ItemChanged += value; }
		//    remove { this._ItemChanged -= value; }
		//}

		///// <summary>
		///// Raises the ItemChanged event.
		///// </summary>
		//protected virtual void OnItemChanged(T oldvalue, T newvalue)
		//{
		//    // if there are event subscribers...
		//    if (this._ItemChanged != null)
		//    {
		//        // call them...
		//        this._ItemChanged(this, oldvalue, newvalue);
		//    }
		//}
		#endregion ItemChanged event

		[Browsable( false )]
		public System.Windows.Forms.ListView Control
		{
			get { return _Control; }
		}

		//public System.Windows.Forms.ListView.ColumnHeaderCollection Columns
		//{
		//    get { this.Synchronize(); return this.ListViewControl.Columns; }
		//    //set { this.ListViewControl.Columns = value; }
		//}

		#endregion Items

		#region SelectedItems

		zeroflag.Collections.Collection<T> _SelectedItems;

		[MergableProperty( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		//[Editor(typeof(System.Windows.Forms.Design.ListViewItemCollectionEditor)
		//"System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
		//[Editor(typeof(zeroflag.Collections.Collection<>), typeof(System.Drawing.Design.UITypeEditor))]
		[Editor( "System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof( System.Drawing.Design.UITypeEditor ) )]
		[Browsable( true )]
		public zeroflag.Collections.Collection<T> SelectedItems
		{
			get { return _SelectedItems ?? ( this._SelectedItems = this.SelectedItemsCreate ); }
		}

		protected virtual zeroflag.Collections.Collection<T> SelectedItemsCreate
		{
			get
			{
				zeroflag.Collections.Collection<T> items = new zeroflag.Collections.Collection<T>();

				items.ItemAdded += __ItemSelected;
				items.ItemRemoved += __ItemDeselected;

				return items;
			}
		}

		void __ItemDeselected( T item )
		{
			this.Synchronize();
			this.OnItemDeselected( item );
		}

		void __ItemSelected( T item )
		{
			this.Synchronize();
			this.OnItemSelected( item );
		}

		#region event ItemSelected
		private event Action<T> _ItemSelected;
		/// <summary>
		/// Item selected
		/// </summary>
		public virtual event Action<T> ItemSelected
		{
			add { this._ItemSelected += value; }
			remove { this._ItemSelected -= value; }
		}
		/// <summary>
		/// Call to raise the ItemSelected event:
		/// Item selected
		/// </summary>
		protected virtual void OnItemSelected( T item )
		{
			// if there are event subscribers...
			if ( this._ItemSelected != null )
			{
				// call them...
				this._ItemSelected( item );
			}
		}
		#endregion event ItemSelected

		#region event ItemDeselected
		private event Action<T> _ItemDeselected;
		/// <summary>
		/// Item deselected.
		/// </summary>
		public virtual event Action<T> ItemDeselected
		{
			add { this._ItemDeselected += value; }
			remove { this._ItemDeselected -= value; }
		}
		/// <summary>
		/// Call to raise the ItemDeselected event:
		/// Item deselected.
		/// </summary>
		protected virtual void OnItemDeselected( T item )
		{
			// if there are event subscribers...
			if ( this._ItemDeselected != null )
			{
				// call them...
				this._ItemDeselected( item );
			}
		}
		#endregion event ItemDeselected

		#endregion SelectedItems

		#region Synchronization

		#region ItemSync

		private zeroflag.Collections.CollectionSynchronizer<T, ListViewItem<ListView<T>, T>> _ItemSync = null;

		public zeroflag.Collections.CollectionSynchronizer<T, ListViewItem<ListView<T>, T>> ItemSync
		{
			get { return _ItemSync ?? ( _ItemSync = this.ItemSyncCreate ); }
			set
			{
				if ( _ItemSync != value )
				{
					_ItemSync = value;
				}
			}
		}

		protected virtual zeroflag.Collections.CollectionSynchronizer<T, ListViewItem<ListView<T>, T>> ItemSyncCreate
		{
			get
			{
				return new zeroflag.Collections.CollectionSynchronizer<T, ListViewItem<ListView<T>, T>>
					( this.Items,
					item => new ListViewItem<ListView<T>, T>( this, item ),
					view => this.Control.Items.Remove( view ),
					( item, view ) =>
					{
						view.Synchronize();
					}
					);
			}
		}

		#endregion ItemSync

		#region SelectedItemSync

		private zeroflag.Collections.CollectionSynchronizer<T, ListViewItem<ListView<T>, T>> _SelectedItemSync = null;

		public zeroflag.Collections.CollectionSynchronizer<T, ListViewItem<ListView<T>, T>> SelectedItemSync
		{
			get { return _SelectedItemSync ?? ( _SelectedItemSync = this.SelectedItemSyncCreate ); }
			set
			{
				if ( _SelectedItemSync != value )
				{
					_SelectedItemSync = value;
				}
			}
		}

		protected virtual zeroflag.Collections.CollectionSynchronizer<T, ListViewItem<ListView<T>, T>> SelectedItemSyncCreate
		{
			get
			{
				return new zeroflag.Collections.CollectionSynchronizer<T, ListViewItem<ListView<T>, T>>
					( this.SelectedItems,
					item => { var view = this.ItemSync[ item ]; view.Selected = true; return view; },
					view => view.Selected = false,
					( item, view ) =>
					{
						view.Synchronize();
					}
					);
			}
		}

		#endregion SelectedItemSync

		#region ColumnSync

		private zeroflag.Collections.CollectionSynchronizer<PropertyDescription, ColumnHeader> _ColumnSync = null;

		public zeroflag.Collections.CollectionSynchronizer<PropertyDescription, ColumnHeader> ColumnSync
		{
			get { return _ColumnSync ?? ( _ColumnSync = this.ColumnSyncCreate ); }
			set
			{
				if ( _ColumnSync != value )
				{
					_ColumnSync = value;
				}
			}
		}

		protected virtual zeroflag.Collections.CollectionSynchronizer<PropertyDescription, ColumnHeader> ColumnSyncCreate
		{
			get
			{
				return new zeroflag.Collections.CollectionSynchronizer<PropertyDescription, ColumnHeader>
					( this.TypeDescription.Properties,
					prop =>
					{
						if ( !prop.Visible )
							return null;
						ColumnHeader col = new ColumnHeader( prop.Name );
						col.Name = prop.Name;
						col.Text = prop.Name;
						if ( prop.Index != null && prop.Index < this.Control.Columns.Count )
							this.Control.Columns.Insert( prop.Index.Value, col );
						else
							this.Control.Columns.Add( col );
						//col.Width = 0;
						return col;
					},
					col => this.Control.Columns.Remove( col ),
					( prop, col ) =>
					{
						col.Text = prop.Name;
						int old = col.Width;
						bool keep = true;
						int width = 0;
						col.AutoResize( ColumnHeaderAutoResizeStyle.ColumnContent );
						width = Math.Max( width, col.Width );
						if ( old == width )
							keep = false;
						col.AutoResize( ColumnHeaderAutoResizeStyle.HeaderSize );
						width = Math.Max( width, col.Width );
						if ( old == width )
							keep = false;
						if ( !keep )
							col.Width = width;
					}
					);
			}
		}

		#endregion ColumnSync

		/// <summary>
		/// Gets or sets a value indicating whether multiple items can be selected.
		/// </summary>
		/// <value>true if multiple items in the control can be selected at one time; otherwise, false. The default is true.</value>
		public virtual bool MultiSelect
		{
			get
			{
				return this.Control != null && this.Control.MultiSelect;
			}
			set
			{
				if ( this.Control != null )
					this.Control.MultiSelect = value;
			}
		}

		public virtual void Synchronize()
		{
			this.ColumnSync.Synchronize();

			this.ItemSync.Synchronize();

			this.SelectedItemSync.Synchronize();

			//List<T> items = new List<T>(this.Items);
			//List<T> removes = new List<T>();

			//foreach (T item in items)
			//{
			//    this.SynchronizeItem(item);
			//}

		}

		//protected virtual void SynchronizeItem(T item)
		//{
		//    ListViewItem<ListView<T>, T> view;
		//    if (!this.ItemViews.ContainsKey(item))
		//    {
		//        // create view...
		//        view = new ListViewItem<ListView<T>, T>(this, item);

		//        this.ItemViews.Add(item, view);
		//    }
		//    else
		//        view = this.ItemViews[item];
		//}

		//Dictionary<T, ListViewItem<ListView<T>, T>> _ItemViews = new Dictionary<T, ListViewItem<ListView<T>, T>>();

		//protected Dictionary<T, ListViewItem<ListView<T>, T>> ItemViews
		//{
		//    get { return _ItemViews; }
		//}

		//Dictionary<ListViewItem<ListView<T>, T>, T> _ItemValues = new Dictionary<ListViewItem<ListView<T>, T>, T>();

		//protected Dictionary<ListViewItem<ListView<T>, T>, T> ItemValues
		//{
		//    get { return _ItemValues; }
		//}

		#endregion Synchronization

		protected override void OnInvalidated( InvalidateEventArgs e )
		{
			this.Synchronize();
			base.OnInvalidated( e );
		}

		protected override void OnVisibleChanged( EventArgs e )
		{
			base.OnVisibleChanged( e );
			if ( this.Visible )
				this.Synchronize();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
			this.Synchronize();
		}
	}
}
