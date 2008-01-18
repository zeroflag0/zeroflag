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