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