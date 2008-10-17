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
	public partial class TreeEditor<T>
		: TreeView<T>
		where T : class
	{
		//class T { }

		//class TreeView : TreeView<T> { }

		public TreeEditor()
		{
			InitializeComponent();
		}


		#region FilterAvailableTypes
		private FilterTypeHandler<T> _FilterAvailableTypes;

		/// <summary>
		/// A filter that can be used to limit available types for adding.
		/// </summary>
		public FilterTypeHandler<T> FilterAvailableTypes
		{
			get { return _FilterAvailableTypes ?? ( _FilterAvailableTypes = this.FilterAvailableTypesCreate ); }
			set { _FilterAvailableTypes = value; }
		}

		/// <summary>
		/// Creates the default/initial value for FilterAvailableTypes.
		/// A filter that can be used to limit available types for adding.
		/// </summary>
		protected virtual FilterTypeHandler<T> FilterAvailableTypesCreate
		{
			get { return ( item, type ) => FilterVisibility.Enabled; }
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



		void contextMenu_Opening( object sender, System.ComponentModel.CancelEventArgs e )
		{
			contextMenu.Enabled = this.SelectedNode != null;
		}

		void treeView_NodeMouseClick( object sender, System.Windows.Forms.TreeNodeMouseClickEventArgs e )
		{
			if ( e.Button == MouseButtons.Right && this.SelectedNode != null )
			{
				this.SelectedNode = e.Node as TreeViewItem<T>;
				//this.contextMenu.Show( e.X, e.Y );
			}
		}
		Point locationLastContext;
		private void contextAdd_DropDownOpening( object sender, EventArgs e )
		{
			this.contextAdd.DropDownItems.Clear();
			this.contextAdd.DropDownItems.Add( this.loadingToolStripMenuItem );
			this.loadingToolStripMenuItem.Visible = true;
			//this.contextAdd.Width = this.buttonAdd.Width - 4;
			//this.contextAdd.Show( this.buttonAdd, 2, 2 );
			this.contextMenu.CreateControl();
			locationLastContext = new Point( this.contextMenu.Left, this.contextMenu.Top );

			this.taskProcessor.Tasks.Add( new Action( () =>
			{
				List<Type> types = zeroflag.Reflection.TypeHelper.GetDerived( typeof( T ) );
				foreach ( var type in types )
				{
					if ( type.IsAbstract || type.IsInterface )
						continue;
					this.BeginInvoke( new Action<Type>( t =>
					{
						ToolStripMenuItem item = new ToolStripMenuItem();
						switch ( this.FilterAvailableTypes( this.SelectedNode.Value, t ) )
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
						this.contextAdd.DropDownItems.Add( item );
					} ), type );
				}
				this.BeginInvoke( new Action( () =>
				{
					this.loadingToolStripMenuItem.Visible = false;
				} ) );
			} ) );
		}

		void AddItemClick( object sender, EventArgs e )
		{
			this.taskProcessor.Tasks.Add( new Action( () =>
			{
				ToolStripMenuItem item = sender as ToolStripMenuItem;
				Type type = item.Tag as Type;
				T instance = (T)zeroflag.Reflection.TypeHelper.CreateInstance( type );
				this.BeginInvoke( new Action( () =>
				{
					this.SelectedNode.Add( instance );
					this.Synchronize();
				} ) );
			} ) );
		}

		void contextRemove_Click( object sender, System.EventArgs e )
		{
			if ( this.contextRemoveConfirm.Items.Count <= 0 )
			{
				this.contextRemoveConfirm.Items.Add( new ToolStripMenuItem( "Really remove this item?" ) { Enabled = false } );
				var cancel = new ToolStripMenuItem( " Cancel" );
				cancel.Click += ( cs, ce ) => { this.contextRemoveConfirm.Close(); };
				this.contextRemoveConfirm.Items.Add( cancel );

				var confirm = new ToolStripMenuItem( " Confirm" );
				confirm.Click += ( cs, ce ) =>
				{
					this.contextRemoveConfirm.Close();
					if ( this.SelectedNode.Parent != null )
						( this.SelectedNode.Parent as TreeViewItem<T> ).Remove( this.SelectedNode.Value );
					this.Synchronize();
				};
				this.contextRemoveConfirm.Items.Add( confirm );
			}
			this.contextRemoveConfirm.Show( this.locationLastContext );
			//this.PointToClient( 
			//new Point( 
			//    Form.MousePosition.X - this.contextRemoveConfirm.Items[0].Height, 
			//    Form.MousePosition.Y - 5 ) 
			//    ) 
			//    );
		}
	}
}
