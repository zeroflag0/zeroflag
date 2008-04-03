using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Test
{
	public partial class DebugForm : Form
	{
		public object Target
		{
			get { return this.propertyGrid.SelectedObject; }
			set { this.propertyGrid.SelectedObject = value; }
		}

		public DebugForm()
		{
			InitializeComponent();
		}
	}
}
