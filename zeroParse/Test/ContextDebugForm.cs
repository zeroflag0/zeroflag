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
		#region ParserThread

		private System.Threading.Thread _ParserThread = default(System.Threading.Thread);

		public System.Threading.Thread ParserThread
		{
			get { return _ParserThread; }
			set
			{
				if (_ParserThread != value)
				{
					_ParserThread = value;
				}
			}
		}
		#endregion ParserThread

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
			//this.treeView.Nodes.Clear();
			try
			{
				TreeNode node = this.Parse(context, null, 0);

				this.progress.Value = 0;
				this.progress.Maximum = context.InnerDepth;

				if (!this.treeView.Nodes.Contains(node) && !this.treeView.Nodes.ContainsKey(node.Text))
					this.treeView.Nodes.Add(node);
				//node.Expand();
				foreach (TreeNode inner in node.Nodes)
					inner.Expand();
			}
			catch (Exception exc)
			{
				Console.WriteLine("Show(): " + exc);
				this.treeView.Nodes.Add(exc.ToString());
			}
		}

		public void Show(ICollection<zeroParse.ParserContext> context)
		{
			this.Show();
			//this.treeView.Nodes.Clear();
			try
			{
				TreeNode node = this.Parse(context);
				if (!this.treeView.Nodes.Contains(node) && !this.treeView.Nodes.ContainsKey(node.Text))
					this.treeView.Nodes.Add(node);
				//node.Expand();
				foreach (TreeNode inner in node.Nodes)
					inner.Collapse();
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

				if (this.progress.Maximum < depth)
					this.progress.Maximum = depth + 1;
				this.progress.Value = depth;
				//Console.Write("node: " + context + "\r");
				Application.DoEvents();
				zeroParse.ParserContext named = null;// this.FindNamed(context);

				TreeNode node = new TreeNode();
				bool redecorate = true;
				if (parent != null)
					foreach (TreeNode peer in parent.Nodes)
						if (peer != null && peer.Tag == context)
						{
							node = peer;
							parent = node.Parent;
							redecorate = false;
							break;
						}

				if (context.Outer != null && context.Outer.Rule is zeroParse.Chain && context.Rule is zeroParse.Chain && parent != null ||
					context.Outer != null && context.Outer.Rule is zeroParse.Or && context.Rule is zeroParse.Or && parent != null ||
					parent != null && parent.Tag == context)
				{
					node = parent;

					redecorate = false;
				}
				if (depth < 5)
				{
					if (parent != null && parent != node)
					{
						if (!parent.Nodes.Contains(node) && !parent.Nodes.ContainsKey(node.Text))
							parent.Nodes.Add(node);
					}
					//else 
					if (parent == null)
					{
						//if (!this.treeView.Nodes.Contains(node) && !this.treeView.Nodes.ContainsKey(node.Text))
						//    this.treeView.Nodes.Add(node);
						node.Expand();
					}
				}
				string text = "";

				if (context.Rule != null)
				{
					text += "" + (context.Result ?? (object)(context.Rule.Name + " " + context.Rule));
				}
				else
					text += "<null>";


				if (parent == null)
					text = "'" + context.Context + "'" + text;
				if (redecorate)
				{
					if (context.Success)
					{
						node.BackColor = Color.FromArgb(100, Color.LightGreen);
						if (depth > 4)
							context.Trim();
						//text += " success";
					}
					else
					{
						node.BackColor = Color.FromArgb(100, Color.Red);
						text += " FAILED";
					}
					if (node != parent)
					{
						node.Tag = context;

						List<TreeNode> empty = new List<TreeNode>();
						foreach (TreeNode item in node.Nodes)
							if (item.Tag as zeroParse.ParserContext == null)
								empty.Add(item);
						foreach (TreeNode item in empty)
							node.Nodes.Remove(item);

						if (named != null && named != context)
						{
							text = "[" + named.Rule.Name + "] " + text;
							if (named.Rule.Ignore)
								return node;

							TreeNode namedNode = new TreeNode("[" + named.Rule.Name + "] " + named.Result);
							node.Nodes.Insert(0, namedNode);
							//TreeNode namedNode = this.Parse(named, null, depth);
							//if (namedNode != null)
							//{
							//    namedNode.Text = "named=" + namedNode.Text;
							//    if (!node.Nodes.Contains(namedNode) && !node.Nodes.ContainsKey(namedNode.Text))
							//        node.Nodes.Add(namedNode);
							//}
						}
						node.Nodes.Insert(0, "line=" + context.Line + ", source=" + context.ToString());
						{
							TreeNode resultNode = new TreeNode("result=" + (context.Result ?? (object)"<null>"));
							resultNode.Tag = context.Result;
							node.Nodes.Insert(0, resultNode);
						}
						if (context.Rule != null)
						{
							string structure = context.Rule.Structure.Replace("\0", @"\0").Replace("\r\n", "\n");
							if (context.Rule.Name != null)
								text = "['" + context.Rule.Name + "']" + text;
							TreeNode rulenode = new TreeNode("rule=" + context.Rule.GetType().Name + (context.Rule.Name != null ? "['" + context.Rule.Name + "']" : "") + ":=" + structure);
							rulenode.Nodes.Insert(0, this.Parse(context.Rule, 0));
							node.Nodes.Insert(0, rulenode);

						}
					}

					text = text.Replace("\r", "").Replace("\0", "");
					if (text.Length < 20)
					{
						text = text.PadRight(20);
						text = text.Replace("\n", "\\n");
					}
					node.Text = text;
				}
				if (!(context.Rule is zeroParse.Whitespace) && depth < MaxDepth || node == parent)
				{
					foreach (zeroParse.ParserContext inner in context.Inner)
					{
						TreeNode sub = null;

						foreach (TreeNode peer in node.Nodes)
							if (peer != null && peer.Tag == inner)
							{
								sub = peer;
								break;
							}
						if (sub != null)
						{
							this.RebuildNode(sub);
						}
						else
						{
							sub = this.Parse(inner, node, depth + 1);

							if (sub != null && !object.ReferenceEquals(node, sub) && !node.Nodes.Contains(sub))
								node.Nodes.Add(sub);
						}
						//else
						//    Console.WriteLine("Skipping node " + sub);
					}
				}
				else
				{

					TreeNode demand = new TreeNode("...");
					demand.Tag = "load";
					node.Nodes.Add(demand);
				}

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
		const int MaxDepth = 10;

		TreeNode Parse(ICollection<zeroParse.ParserContext> context)
		{
			int depth = 0;
			if (closing)
				return null;
			try
			{
				//Console.Write("node: " + context + "\r");
				Application.DoEvents();
				//this.treeView.SuspendLayout();
				TreeNode node = new TreeNode("Trace[" + context.Count + "]");

				//if (this.treeView.Nodes.Count < 1)
				//{
				//    if (!this.treeView.Nodes.Contains(node) && !this.treeView.Nodes.ContainsKey(node.Text))
				//        this.treeView.Nodes.Add(node);
				//}

				foreach (var inner in context)
				{
					TreeNode sub = this.Parse(inner, node, depth + 1);
					if (sub != null && sub != node && !node.Nodes.Contains(sub))
						node.Nodes.Add(sub);
				}
				//this.treeView.ResumeLayout();
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

		TreeNode Parse(zeroParse.Rule rule, int depth)
		{
			try
			{
				Application.DoEvents();
				TreeNode node = new TreeNode();
				node.Text = "" + rule;

				if (rule.Name != null)
					node.BackColor = Color.FromArgb(100, Color.LightSkyBlue);
				//else
				//    node.Expand();
				if (!rule.Primitive && !rule.Ignore && depth < 5)
					foreach (var inner in rule)
					{
						if (inner != null)
							node.Nodes.Add(this.Parse(inner, depth + 1));
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

		void ExpandSuccess(TreeNode node)
		{
			this.ExpandSuccess(node, MaxDepth - 2);
		}
		void ExpandSuccess(TreeNode node, int depth)
		{
			depth++;
			if (node == null || depth > MaxDepth || !(node.Tag is zeroParse.ParserContext) ||
				((zeroParse.ParserContext)node.Tag).Rule == null || (((zeroParse.ParserContext)node.Tag).Rule is zeroParse.Whitespace) ||
				((zeroParse.ParserContext)node.Tag).Rule.Ignore || ((zeroParse.ParserContext)node.Tag).Rule.Primitive
				)
				return;
			if (node.Tag is zeroParse.ParserContext && ((zeroParse.ParserContext)node.Tag).Success)
			{
				node.Expand();
				this.treeView.SelectedNode = node;
				foreach (TreeNode inner in node.Nodes)
					this.ExpandSuccess(inner, depth);
			}
		}

		void FlattenPeers(TreeNode node)
		{
			if (node == null)
				return;
			node = node.Parent ?? node;
			this.treeView.SuspendLayout();
			node.Expand();
			foreach (TreeNode inner in node.Nodes)
				inner.Collapse();
			this.treeView.ResumeLayout();
		}

		void RebuildNode(TreeNode node)
		{
			if (node != null)
			{
				bool expand = node.IsExpanded;
				node.Collapse();
				//node.Nodes.Clear();
				TreeNode demand = new TreeNode("...");
				demand.Tag = "load";
				node.Nodes.Add(demand);
				if (expand)
					node.Expand();
			}
		}

		private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
		{
			TreeNode selected = e.Node;
			if (selected == null)
				return;
			TreeNode last = null;
			if (((selected.Tag + "") == "load" || selected.Text == "...") && selected.Parent != null)
			{
				Console.WriteLine("Selecting parent for load...");
				last = selected;
				selected = selected.Parent;
			}

			//if (last == null)
			foreach (TreeNode node in selected.Nodes)
				if ((node.Tag + "") == "load" || node.Text == "...")
					last = node;

			if (last != null)
			{
				Console.WriteLine("Loading nodes for " + selected.Text);
				selected.Nodes.Remove(last);
				this.Parse(selected.Tag as zeroParse.ParserContext, selected.Parent, 0);
			}
			//else
			//    Console.WriteLine("Couldn't find a node to load... " + selected.Tag);
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			this.treeView.SelectedNode.Expand();
		}

		private void treeView_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F4)
				this.ExpandSuccess(this.treeView.SelectedNode);
			else if (e.KeyCode == Keys.F3)
				this.FlattenPeers(this.treeView.SelectedNode);
			else if (e.KeyCode == Keys.F5)
				this.RebuildNode(this.treeView.SelectedNode);
			else if (e.KeyCode == Keys.F9)
			{
				List<zeroParse.ParserContext> contexts = new List<zeroParse.ParserContext>();
				foreach (TreeNode node in this.treeView.Nodes)
					if (node.Tag is zeroParse.ParserContext)
						contexts.Add(node.Tag as zeroParse.ParserContext);
				this.treeView.Nodes.Clear();
				foreach (var cont in contexts)
					this.Show(cont);
			}
			else if (e.KeyCode == Keys.F1)
				Console.WriteLine(this.treeView.SelectedNode);
			else if (e.KeyCode == Keys.F11)
			{
				try
				{
					this.ParserThread.Abort();
				}
				catch (Exception exc)
				{
					Console.WriteLine(exc);
				}
			}
		}

		private void treeView_MouseClick(object sender, MouseEventArgs e)
		{
			if (this.treeView.SelectedNode != null)
			{
				if (e.Button == MouseButtons.Right)
				{
					this.CollapseOne(this.treeView.SelectedNode);
				}
				else if (e.Button == MouseButtons.Middle)
				{
					this.ExpandSuccess(this.treeView.SelectedNode);
				}
				else
				{
					if (this.treeView.SelectedNode != null)
						this.treeView.SelectedNode.Expand();
				}
				//else if (e.Button == MouseButtons.Left)
				//{
				//    this.treeView.SelectedNode.Expand();
				//}
			}
		}

		private void treeView_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (this.treeView.SelectedNode != null)
			{
				Console.WriteLine(this.treeView.SelectedNode);
			}
		}
	}
}
