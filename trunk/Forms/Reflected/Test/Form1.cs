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

			this.testList1.ItemSelected += testList1_ItemSelected;
			this.testList1.ItemDeselected += testList1_ItemSelected;
		}

		void testList1_ItemSelected(TestData item)
		{
			this.propertyGrid1.SelectedObjects = this.testList1.SelectedItems.ToArray();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			TestData root = new TestData("root", 1, 1.5f);

			TestData foo = new TestData("foo", 2, 51);
			TestData bar = new TestData2("bar", 3, 0.0005f);//.Add(root);
			//root.Add(foo, bar);
			this.testList1.Items.Add(root);
			this.testList1.Items.Add(foo);
			this.testList1.Items.Add(bar);
		}
	}
}
