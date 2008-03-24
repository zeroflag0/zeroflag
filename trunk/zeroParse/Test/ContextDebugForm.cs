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
	public partial class ContextDebugForm : Form
	{
		public ContextDebugForm(zeroParse.ParserContext context)
			: this()
		{
			this.Show(context);
		}

		public ContextDebugForm()
		{
			InitializeComponent();
		}

		bool closing = false;
		protected override void OnClosed(EventArgs e)
		{
			closing = true;
			base.OnClosed(e);
		}

		public void Show(zeroParse.ParserContext context)
		{
			this.Show();
			this.treeView.Nodes.Clear();
			try
			{
				TreeNode node = this.Parse(context, null, 0);
				if (!this.treeView.Nodes.Contains(node) && !this.treeView.Nodes.ContainsKey(node.Text))
					this.treeView.Nodes.Add(node);
				this.treeView.Nodes[0].Expand();
				foreach (TreeNode inner in this.treeView.Nodes[0].Nodes)
					inner.Expand();
			}
			catch (Exception exc)
			{
				Console.WriteLine("Show(): " + exc);
				this.treeView.Nodes.Add(exc.ToString());
			}
		}

		TreeNode Parse(zeroParse.ParserContext context, TreeNode parent, int depth)
		{
			if (closing)
				return null;
			try
			{
				//Console.Write("node: " + context + "\r");
				Application.DoEvents();
				zeroParse.ParserContext named = null;// this.FindNamed(context);

				TreeNode node = new TreeNode();
				if (context.Outer != null && context.Outer.Rule is zeroParse.Chain && context.Rule is zeroParse.Chain && parent != null ||
					context.Outer != null && context.Outer.Rule is zeroParse.Or && context.Rule is zeroParse.Or && parent != null)
				{
					node = parent;
				}
				if (depth < 3)
				{
					if (parent != null && parent != node)
					{
						if (!parent.Nodes.Contains(node) && !parent.Nodes.ContainsKey(node.Text))
							parent.Nodes.Add(node);
					}
					else if (this.treeView.Nodes.Count < 1)
					{
						if (!this.treeView.Nodes.Contains(node) && !this.treeView.Nodes.ContainsKey(node.Text))
							this.treeView.Nodes.Add(node);
					}
					node.Expand();
				}
				string text = "";

				if (context.Rule != null)
				{
					text += "" + (context.Result ?? (object)(context.Rule.Name + " " + context.Rule));
				}
				else
					text += "<null>";

				if (node != parent)
				{
					if (named != null && named != context)
					{
						text = "[" + named.Rule.Name + "] " + text;
						if (named.Rule.Ignore)
							return node;

						TreeNode namedNode = new TreeNode("[" + named.Rule.Name + "] " + named.Result);
						node.Nodes.Add(namedNode);
						//TreeNode namedNode = this.Parse(named, null, depth);
						//if (namedNode != null)
						//{
						//    namedNode.Text = "named=" + namedNode.Text;
						//    if (!node.Nodes.Contains(namedNode) && !node.Nodes.ContainsKey(namedNode.Text))
						//        node.Nodes.Add(namedNode);
						//}
					}
					if (context.Success)
					{
						node.BackColor = Color.FromArgb(100, Color.LightGreen);
						//text += " success";
					}
					else
					{
						node.BackColor = Color.FromArgb(100, Color.Red);
						text += " FAILED";
					}

					if (context.Rule != null)
					{
						string structure = context.Rule.Structure;
						structure = structure.Replace("\0", @"\0").Replace("\r\n", @"\n").Replace("\n", @"\n");
						if (context.Rule.Name != null)
							text = "['" + context.Rule.Name + "']" + text;
						TreeNode rulenode = new TreeNode("rule=" + context.Rule.GetType().Name + (context.Rule.Name != null ? "['" + context.Rule.Name + "']" : "") + ":=" + structure);
						//rulenode.Nodes.Add(structure);
						node.Nodes.Add(rulenode);

					}
					node.Nodes.Add("result=" + (context.Result ?? (object)"<null>"));
					node.Nodes.Add("line=" + context.Line + ", source=" + context.ToString());
				
					text = text.Replace("\r", "").Replace("\0", "");
					if (text.Length < 20)
					{
						text = text.PadRight(20);
						text = text.Replace("\n", "\\n");
					}
					node.Text = text;
				}
				if (depth < 30)
					foreach (var inner in context.Inner)
					{
						TreeNode sub = this.Parse(inner, node, depth + 1);
						if (sub != null && sub != node && !node.Nodes.Contains(sub))
							node.Nodes.Add(sub);
					}
				else
					node.Nodes.Add("[...]");

				return node;
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
				TreeNode node = new TreeNode(exc.GetType().Name);
				node.Nodes.Add(exc.ToString());
				node.BackColor = Color.FromArgb(150, Color.OrangeRed);
				return node;
			}
		}


		zeroParse.ParserContext FindNamed(zeroParse.ParserContext context)
		{
			if (context != null)
			{
				if (context.Success && context.Rule != null && !context.Rule.Ignore && context.Rule.Name != null)
					return context;
				else
				{
					zeroParse.ParserContext match = null;
					foreach (zeroParse.ParserContext inner in context.Inner)
					{
						match = this.FindNamed(inner);
						if (match != null)
							return match;
					}
				}
			}
			return null;
		}

		private void treeView_MouseClick(object sender, MouseEventArgs e)
		{
			if (this.treeView.SelectedNode != null)
			{
				if (e.Button == MouseButtons.Right)
				{
					this.CollapseOne(this.treeView.SelectedNode);
				}
				//else if (e.Button == MouseButtons.Left)
				//{
				//    this.treeView.SelectedNode.Expand();
				//}
			}
		}

		void CollapseOne(TreeNode node)
		{
			if (node == null)
				return;
			if (node.IsExpanded)
			{
				node.Collapse();
				this.treeView.SelectedNode = node;
			}
			else
				this.CollapseOne(node.Parent);
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			this.treeView.SelectedNode.Expand();
		}
	}
}
