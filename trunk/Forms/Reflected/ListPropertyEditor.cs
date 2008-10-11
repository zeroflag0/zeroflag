using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace zeroflag.Forms.Reflected
{
	public partial class ListPropertyEditor<T> : UserControl
	{
		public ListPropertyEditor()
		{
			InitializeComponent();
		}

		public SplitContainer SplitContainer
		{
			get { return splitContainer; }
		}

		public System.Windows.Forms.PropertyGrid PropertyGrid
		{
			get { return propertyGrid; }
		}

		public ListEditor<T> ListView
		{
			get { return listView; }
		}


		protected virtual void SynchronizeSelected()
		{
			if ( this.listView.SelectedItems.Count > 0 )
			{
				if ( typeof( T ).IsValueType )
				{
					// slow manual boxing for value types...
					List<object> items = new List<object>();
					foreach ( var item in this.listView.SelectedItems )
						items.Add( item );
					this.propertyGrid.SelectedObjects = items.ToArray();
				}
				else
					this.propertyGrid.SelectedObjects = (object[])(object)this.listView.SelectedItems.ToArray();
			}
			else
				this.propertyGrid.SelectedObject = null;
		}

		void listView_ItemRemoved( T obj )
		{
			while ( this.listView.SelectedItems.Contains( obj ) )
				this.listView.SelectedItems.Remove( obj );
			this.SynchronizeSelected();
		}

		void listView_ItemAdded( T obj )
		{
			this.SynchronizeSelected();
		}

		void listView_ItemDeselected( T obj )
		{
			this.SynchronizeSelected();
		}

		void listView_ItemSelected( T obj )
		{
			this.SynchronizeSelected();
		}

		#region Items
		[MergableProperty( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		//[Editor(typeof(System.Windows.Forms.Design.ListViewItemCollectionEditor)
		//"System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
		//[Editor(typeof(zeroflag.Collections.Collection<>), typeof(System.Drawing.Design.UITypeEditor))]
		[Editor( "System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof( System.Drawing.Design.UITypeEditor ) )]
		[Browsable( true )]
		public zeroflag.Collections.List<T> Items
		{
			get { return this.listView.Items; }
			set { this.listView.Items = value; }
		}

		/// <summary>
		/// Item added
		/// </summary>
		public virtual event Action<T> ItemAdded
		{
			add { this.listView.ItemAdded += value; }
			remove { this.listView.ItemAdded -= value; }
		}

		/// <summary>
		/// Item removed.
		/// </summary>
		public virtual event Action<T> ItemRemoved
		{
			add { this.listView.ItemRemoved += value; }
			remove { this.listView.ItemRemoved -= value; }
		}
		#endregion

		#region SelectedItems
		[MergableProperty( false )]
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Content )]
		//[Editor(typeof(System.Windows.Forms.Design.ListViewItemCollectionEditor)
		//"System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
		//[Editor(typeof(zeroflag.Collections.Collection<>), typeof(System.Drawing.Design.UITypeEditor))]
		[Editor( "System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof( System.Drawing.Design.UITypeEditor ) )]
		[Browsable( true )]
		public zeroflag.Collections.Collection<T> SelectedItems
		{
			get { return this.listView.SelectedItems; }
		}

		#region event ItemSelected
		/// <summary>
		/// Item selected
		/// </summary>
		public virtual event Action<T> ItemSelected
		{
			add { this.listView.ItemSelected += value; }
			remove { this.listView.ItemSelected -= value; }
		}
		#endregion event ItemSelected

		#region event ItemDeselected
		/// <summary>
		/// Item deselected.
		/// </summary>
		public virtual event Action<T> ItemDeselected
		{
			add { this.listView.ItemDeselected += value; }
			remove { this.listView.ItemDeselected -= value; }
		}
		#endregion event ItemDeselected

		#endregion SelectedItems

		public TypeDescription TypeDescription
		{
			get { return this.listView.TypeDescription; }
			set { this.listView.TypeDescription = value; }
		}
	}
}
