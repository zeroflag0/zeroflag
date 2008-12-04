using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace zeroflag.Forms
{
	public partial class DebugForm : Form
	{
		public DebugForm( Exception exc, params object[] items )
			: this( "" + ( exc ?? (object)"<null>" ), items )
		{
		}
		public DebugForm( string msg, params object[] items )
			: this( items )
		{
			this.textBox.Text = msg + "\n\n" + this.textBox.Text;
		}
		public DebugForm( params object[] items )
			: this()
		{
			this.AddItems( items );
			this.Show();
		}
		public DebugForm( Control owner, params object[] items )
			: this()
		{
			this.AddItems( items );
			this.Parent = owner;
			this.Show( owner );
		}

		void AddItems( System.Collections.IEnumerable items )
		{
			if ( items == null )
				return;
			if ( items is string )
				return;
			foreach ( object item in items )
			{
				this.listBox.Items.Add( ( item ?? (object)"<null>" ) );
				this.AddItems( item as System.Collections.IEnumerable );
				this.textBox.Text += ( item ?? (object)"<null>" ) + "\n";
			}
		}

		public DebugForm()
		{
			InitializeComponent();
		}

		private void listBox_SelectedIndexChanged( object sender, EventArgs e )
		{
			object[] items = new object[this.listBox.SelectedItems.Count];
			this.listBox.SelectedItems.CopyTo( items, 0 );
			this.propertyGrid.SelectedObjects = items;
		}
	}
}
