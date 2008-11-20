using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Test
{
	public partial class ReportForm : Form
	{
		public string Report
		{
			get { return this.text.Text; }
			set { this.text.Text = value; }
		}

		public ReportForm( string report )
			: this()
		{
			this.Report = report;
		}
		public ReportForm()
		{
			InitializeComponent();
		}
	}
}
