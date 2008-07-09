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
	public partial class TreeEditor<T> : UserControl
		where T : class
	{
		public TreeEditor()
		{
			InitializeComponent();
		}

		private void contextAdd_DropDownOpening( object sender, EventArgs e )
		{
			this.contextAdd.DropDownItems.Clear();
			this.contextAdd.DropDownItems.Add( this.loadingToolStripMenuItem );
			this.loadingToolStripMenuItem.Visible = true;
			//this.contextAdd.Width = this.buttonAdd.Width - 4;
			//this.contextAdd.Show( this.buttonAdd, 2, 2 );

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
					//this.treeView.SelectedNode.Nodes.Add( instance );
				} ) );
			} ) );
		}
	}
}
