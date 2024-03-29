﻿using System;
//using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using zeroflag.Collections;

namespace zeroflag.Forms.Reflected
{
	public partial class ListEditor<T> : UserControl
	{
		public ListEditor()
		{
			InitializeComponent();

			this.backgroundWorker.DoWork += new DoWorkEventHandler( backgroundWorker_DoWork );
			this.backgroundWorker.WorkerSupportsCancellation = true;
		}

		#region FilterAvailableTypes
		private FilterTypeHandler _FilterAvailableTypes;

		/// <summary>
		/// A filter that can be used to limit available types for adding.
		/// </summary>
		public FilterTypeHandler FilterAvailableTypes
		{
			get { return _FilterAvailableTypes ?? ( _FilterAvailableTypes = this.FilterAvailableTypesCreate ); }
			set { _FilterAvailableTypes = value; }
		}

		/// <summary>
		/// Creates the default/initial value for FilterAvailableTypes.
		/// A filter that can be used to limit available types for adding.
		/// </summary>
		protected virtual FilterTypeHandler FilterAvailableTypesCreate
		{
			get { return type => FilterVisibility.Enabled; }
		}

		#endregion FilterAvailableTypes


		#region HideFilteredTypesCompletely

		private bool _HideFilteredTypesCompletely = false;

		public bool HideFilteredTypesCompletely
		{
			get { return _HideFilteredTypesCompletely; }
			set
			{
				if ( _HideFilteredTypesCompletely != value )
				{
					_HideFilteredTypesCompletely = value;
				}
			}
		}
		#endregion HideFilteredTypesCompletely

		private void buttonRemove_Click( object sender, EventArgs e )
		{
			using ( this.listView.DisableSync )
			{
				List<T> remove = new List<T>( this.SelectedItems.ToArray() );
				this.SelectedItems.Clear();
				foreach ( T item in remove )
				{
					this.Items.Remove( item );
				}
			}
			this.listView.Synchronize();
		}

		private void buttonAdd_Click( object sender, EventArgs e )
		{
			this.contextAdd.Items.Clear();
			this.contextAdd.Items.Add( this.loadingToolStripMenuItem );
			this.loadingToolStripMenuItem.Visible = true;
			this.contextAdd.Width = this.buttonAdd.Width - 4;
			this.contextAdd.Show( this.buttonAdd, 2, 2 );

			this.Tasks.Add( () =>
				{
					List<Type> types = zeroflag.Reflection.TypeHelper.GetDerived( typeof( T ) );
					foreach ( var type in types )
					{
						if ( type.IsAbstract || type.IsInterface )
							continue;
						this.BeginInvoke( new Action<Type>( t =>
							{
								ToolStripMenuItem item = new ToolStripMenuItem();
								switch ( this.FilterAvailableTypes( t ) )
								{
									case FilterVisibility.HiddenCompletely:
										item.Visible = false;
										item.Enabled = false;
										break;
									case FilterVisibility.Disabled:
										item.Enabled = false;
										if ( this.HideFilteredTypesCompletely )
											item.Visible = false;
										break;
									default:
										break;
								}
								item.Text = t.Name ?? t.FullName;
								item.Tag = t;
								item.Click += new EventHandler( AddItemClick );
								this.contextAdd.Items.Add( item );
							} ), type );
					}
					this.BeginInvoke( new Action( () =>
						{
							this.loadingToolStripMenuItem.Visible = false;
						} ) );
				} );
		}

		void AddItemClick( object sender, EventArgs e )
		{
			this.Tasks.Add( () =>
				{
					ToolStripMenuItem item = sender as ToolStripMenuItem;
					Type type = item.Tag as Type;
					T instance = (T)zeroflag.Reflection.TypeHelper.CreateInstance( type );
					this.BeginInvoke( new Action( () =>
						{
							this.listView.Items.Add( instance );
						} ) );
				} );
		}


		#region Tasks
		private zeroflag.Collections.List<Action> _Tasks;

		/// <summary>
		/// Tasks to be processed in the background.
		/// </summary>
		public zeroflag.Collections.List<Action> Tasks
		{
			get { return _Tasks ?? ( _Tasks = this.TasksCreate ); }
		}

		/// <summary>
		/// Creates the default/initial value for Tasks.
		/// Tasks to be processed in the background.
		/// </summary>
		protected virtual zeroflag.Collections.List<Action> TasksCreate
		{
			get
			{
				var list = new zeroflag.Collections.List<Action>() { };
				list.ItemAdded += item =>
					{
						this.backgroundWorker.RunWorkerAsync();
					};
				return list;
			}
		}

		#endregion Tasks


		void backgroundWorker_DoWork( object sender, DoWorkEventArgs e )
		{
			while ( this.Tasks.Count > 0 && !e.Cancel )
			{
				if ( this.Tasks[0] != null )
					this.Tasks[0]();
				this.Tasks.RemoveAt( 0 );
			}
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
