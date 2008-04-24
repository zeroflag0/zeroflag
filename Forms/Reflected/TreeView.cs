using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace zeroflag.Forms.Reflected
{
	public partial class TreeView<T> : UserControl
	{
		public TreeView()
		{
			InitializeComponent();
		}

		[Browsable(false)]
		public System.Windows.Forms.ListView Control
		{
			get { return _Control; }
		}
	}
}
