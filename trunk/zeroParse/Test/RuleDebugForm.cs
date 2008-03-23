using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test
{
	public partial class RuleDebugForm : Form
	{
		public RuleDebugForm(zeroParse.Rule rule)
			: this()
		{
			this.Show(rule);
		}

		public RuleDebugForm()
		{
			InitializeComponent();
		}

		public void Show(zeroParse.Rule rule)
		{
			this.treeView.Nodes.Clear();
			this.treeView.Nodes.Add(this.Parse(rule));
			this.treeView.Nodes[0].Expand();
			this.Show();
		}

		TreeNode Parse(zeroParse.Rule rule)
		{
			try
			{
				TreeNode node = new TreeNode();
				node.Text = "" + rule;

				if (rule.Name != null)
					node.BackColor = Color.FromArgb(100, Color.LightSkyBlue);
				else
					node.Expand();
				foreach (var inner in rule)
				{
					if (inner != null)
						node.Nodes.Add(this.Parse(inner));
				}

				return node;
			}
			catch (Exception exc)
			{
				TreeNode node = new TreeNode(exc.GetType().Name);
				node.Nodes.Add(exc.ToString());
				node.BackColor = Color.FromArgb(150, Color.Red);
				return node;
			}
		}
	}
}
