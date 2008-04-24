using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace zeroflag.Forms.Reflected
{
	public class TreeViewItem<Tree, T> : System.Windows.Forms.TreeNode
		where Tree : TreeView<T>
	{
	}
}
