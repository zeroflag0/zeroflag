using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Test
{
	public partial class TestForm : Form
	{
		public TestForm()
		{
			InitializeComponent();
		}

		public TestForm(TestForm parent)
			: this()
		{
			this.TestParent = parent;
		}

		TestForm _TestParent = null;

		public TestForm TestParent
		{
			get { return _TestParent; }
			set { _TestParent = value; }
		}

		List<TestForm> _TestChildren = new List<TestForm>();

		public List<TestForm> TestChildren
		{
			get { return _TestChildren; }
			set { _TestChildren = value; }
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);

			TestForm child = new TestForm(this);
			this.TestChildren.Add(child);
			child.Show();
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			if (this.TestParent == null)
			{
				zeroflag.Serialization.XmlSerializer seri = new zeroflag.Serialization.XmlSerializer("test_form.xml");
				seri.Serialize(this);
			}
			else
			{
				this.TestParent.TestChildren.Remove(this);
				this.TestParent = null;
			}
		}
	}
}