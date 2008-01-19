#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2007  Thomas "zeroflag" Kraemer
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

namespace zeroflag.Windows
{
	public partial class PropertyMessageBox: Form
	{
		public object Value
		{
			get { return this.propertyGrid.SelectedObject; }
			set { this.propertyGrid.SelectedObject = value; }
		}

		protected PropertyMessageBox()
		{
			InitializeComponent();

			this.DialogResult = DialogResult.OK;
		}

		public PropertyMessageBox(Exception exc)
			: this()
		{
			this.Value = exc;
			this.Text = exc.Message;
			this.label1.Text = exc.ToString();
		}

		public PropertyMessageBox(object value)
			: this()
		{
			this.Value = value;
			this.Text = value.GetType().ToString();
			this.label1.Text = value.ToString();
		}

		public PropertyMessageBox(string caption, object value)
			: this()
		{
			this.Value = value;
			this.Text = caption;
			this.label1.Text = value.ToString();
		}

		public PropertyMessageBox(string caption, string message, object value)
			: this()
		{
			this.Value = value;
			this.Text = caption;
			this.label1.Text = message;
		}

		public static DialogResult Show(Exception exc)
		{
			return new PropertyMessageBox(exc).ShowDialog();
		}

		public static DialogResult Show(object value)
		{
			return new PropertyMessageBox(value).ShowDialog();
		}

		public static DialogResult Show(string message, object value)
		{
			return new PropertyMessageBox(message, value).ShowDialog();
		}

		public static DialogResult Show(string caption, string message, object value)
		{
			return new PropertyMessageBox(caption, message, value).ShowDialog();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}