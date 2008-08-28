using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Test
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			this.testTree1.SelectedItemChanged += new zeroflag.Forms.Reflected.TreeView<TestData>.SelectedItemChangedHandler( testTree1_SelectedItemChanged );
		}

		void testTree1_SelectedItemChanged( object sender, TestData oldvalue, TestData newvalue )
		{
			this.propertyGrid1.SelectedObject = newvalue;
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			var data = new TestData( "root", 0, 1.5f );
			data.Inner.Add( new TestData2() { Name = "test2" } );
			data.Inner.Add( new TestData() { Name = "test" } );
			this.testTree1.Node = data;
		}
	}
}
