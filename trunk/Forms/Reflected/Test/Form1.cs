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

			this.testTree1.SelectedItemChanged += new zeroflag.Forms.Reflected.TreeView<object>.SelectedItemChangedHandler( testTree1_SelectedItemChanged );
		}

		void testTree1_SelectedItemChanged( object sender, object oldvalue, object newvalue )
		{
			this.propertyGrid1.SelectedObject = newvalue;
			if ( newvalue != null && !newvalue.GetType().IsClass )
				this.propertyGrid1.SelectedObject = new Wrapper() { Value = newvalue };
		}
		class Wrapper
		{
			object _Value;

			public object Value
			{
				get { return _Value; }
				set { _Value = value; }
			}
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );

			var data = new TestData( "root", 0, 1.5f );
			data.Inner.Add( new TestData() { Name = "test1" }.Add( new TestData() { Name = "test1.1" } ) );
			data.Inner.Add( new TestData2() { Name = "test2" }.Add( data ) );

			this.testTree1.Node = data;

			this.testTree1.AutoSynchronize = true;
		}
	}
}
