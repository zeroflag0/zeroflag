#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2008  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************
#endregion LGPL License

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