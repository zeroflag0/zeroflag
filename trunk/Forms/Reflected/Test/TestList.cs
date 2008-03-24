using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Test
{
	public partial class TestList : zeroflag.Forms.Reflected.ListView<TestData>
	{
		public TestList()
		{
			InitializeComponent();
		}

		[MergableProperty(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		//[Editor(typeof(System.Windows.Forms.Design.ListViewItemCollectionEditor)
		//"System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
		[Editor(typeof(TestListCollection), typeof(System.Drawing.Design.UITypeEditor))]
		//[Editor("System.Windows.Forms.Design.ListViewItemCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
		[Browsable(true)]
		public new TestListCollection Items
		{
			get
			{
				return (TestListCollection)base.Items;
			}
		}

		protected override zeroflag.Collections.Collection<TestData> ItemsCreate
		{
			get
			{
				return new TestListCollection();
			}
		}
	}

	public class TestListCollection : zeroflag.Collections.Collection<TestData>
	{
	}
}
