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
	public partial class SelectWindowDialog : Form
	{
		public SelectWindowDialog()
		{
			InitializeComponent();
			this.DialogResult = DialogResult.Cancel;
		}

		IntPtr m_Target;

		public IntPtr Target
		{
			get { return m_Target; }
			set { m_Target = value; }
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.CreateControl();
			this.Show();
			this.Activate();
		}

		private void SelectWindowDialog_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			}
		}

		protected override void OnDeactivate(EventArgs e)
		{
			base.OnDeactivate(e);

			Application.DoEvents();
			System.Threading.Thread.Sleep(10);
			Application.DoEvents();

			IntPtr handle = zeroflag.Windows.WinAPI.GetForegroundWindow();
			if (handle != this.Handle && handle != this.Owner.Handle)
			{
				this.Target = handle;
				this.DialogResult = DialogResult.OK;
			}
			else
			{
				this.DialogResult = DialogResult.Cancel;
			}
			this.Close();
		}
	}
}