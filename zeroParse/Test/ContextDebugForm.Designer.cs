namespace Test
{
	partial class ContextDebugForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.treeView = new System.Windows.Forms.TreeView();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.progress = new System.Windows.Forms.ToolStripProgressBar();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.nodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.printToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.flattenPeersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.expandSuccessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.rebuildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.trimNodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.trimEverythingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.splitContainer1 = new zeroflag.Forms.SplitContainer();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.statusStrip.SuspendLayout();
			this.menuStrip.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.Font = new System.Drawing.Font( "Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte)( 0 ) ) );
			this.treeView.FullRowSelect = true;
			this.treeView.HideSelection = false;
			this.treeView.Location = new System.Drawing.Point( 0, 0 );
			this.treeView.Name = "treeView";
			this.treeView.PathSeparator = ".";
			this.treeView.Size = new System.Drawing.Size( 599, 791 );
			this.treeView.TabIndex = 0;
			this.treeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler( this.treeView_MouseDoubleClick );
			this.treeView.MouseClick += new System.Windows.Forms.MouseEventHandler( this.treeView_MouseClick );
			this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler( this.treeView_AfterSelect );
			this.treeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler( this.treeView_AfterExpand );
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.progress} );
			this.statusStrip.Location = new System.Drawing.Point( 0, 817 );
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size( 902, 22 );
			this.statusStrip.TabIndex = 1;
			this.statusStrip.Text = "statusStrip1";
			// 
			// progress
			// 
			this.progress.Name = "progress";
			this.progress.Size = new System.Drawing.Size( 400, 16 );
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.nodeToolStripMenuItem} );
			this.menuStrip.Location = new System.Drawing.Point( 0, 0 );
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size( 902, 24 );
			this.menuStrip.TabIndex = 2;
			this.menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem} );
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size( 37, 20 );
			this.fileToolStripMenuItem.Text = "File";
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ( (System.Windows.Forms.Keys)( ( System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S ) ) );
			this.saveToolStripMenuItem.Size = new System.Drawing.Size( 138, 22 );
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler( this.saveToolStripMenuItem_Click );
			// 
			// nodeToolStripMenuItem
			// 
			this.nodeToolStripMenuItem.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.printToolStripMenuItem,
            this.flattenPeersToolStripMenuItem,
            this.expandSuccessToolStripMenuItem,
            this.rebuildToolStripMenuItem,
            this.trimNodeToolStripMenuItem,
            this.trimEverythingToolStripMenuItem} );
			this.nodeToolStripMenuItem.Name = "nodeToolStripMenuItem";
			this.nodeToolStripMenuItem.Size = new System.Drawing.Size( 48, 20 );
			this.nodeToolStripMenuItem.Text = "Node";
			// 
			// printToolStripMenuItem
			// 
			this.printToolStripMenuItem.Name = "printToolStripMenuItem";
			this.printToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.printToolStripMenuItem.Size = new System.Drawing.Size( 177, 22 );
			this.printToolStripMenuItem.Text = "Print";
			this.printToolStripMenuItem.Click += new System.EventHandler( this.printToolStripMenuItem_Click );
			// 
			// flattenPeersToolStripMenuItem
			// 
			this.flattenPeersToolStripMenuItem.Name = "flattenPeersToolStripMenuItem";
			this.flattenPeersToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.flattenPeersToolStripMenuItem.Size = new System.Drawing.Size( 177, 22 );
			this.flattenPeersToolStripMenuItem.Text = "Flatten Peers";
			this.flattenPeersToolStripMenuItem.Click += new System.EventHandler( this.flattenPeersToolStripMenuItem_Click );
			// 
			// expandSuccessToolStripMenuItem
			// 
			this.expandSuccessToolStripMenuItem.Name = "expandSuccessToolStripMenuItem";
			this.expandSuccessToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
			this.expandSuccessToolStripMenuItem.Size = new System.Drawing.Size( 177, 22 );
			this.expandSuccessToolStripMenuItem.Text = "Expand Success";
			this.expandSuccessToolStripMenuItem.Click += new System.EventHandler( this.expandSuccessToolStripMenuItem_Click );
			// 
			// rebuildToolStripMenuItem
			// 
			this.rebuildToolStripMenuItem.Name = "rebuildToolStripMenuItem";
			this.rebuildToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.rebuildToolStripMenuItem.Size = new System.Drawing.Size( 177, 22 );
			this.rebuildToolStripMenuItem.Text = "Rebuild";
			this.rebuildToolStripMenuItem.Click += new System.EventHandler( this.rebuildToolStripMenuItem_Click );
			// 
			// trimNodeToolStripMenuItem
			// 
			this.trimNodeToolStripMenuItem.Name = "trimNodeToolStripMenuItem";
			this.trimNodeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
			this.trimNodeToolStripMenuItem.Size = new System.Drawing.Size( 177, 22 );
			this.trimNodeToolStripMenuItem.Text = "Trim Node";
			this.trimNodeToolStripMenuItem.Click += new System.EventHandler( this.trimNodeToolStripMenuItem_Click );
			// 
			// trimEverythingToolStripMenuItem
			// 
			this.trimEverythingToolStripMenuItem.Name = "trimEverythingToolStripMenuItem";
			this.trimEverythingToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F9;
			this.trimEverythingToolStripMenuItem.Size = new System.Drawing.Size( 177, 22 );
			this.trimEverythingToolStripMenuItem.Text = "Trim Everything";
			this.trimEverythingToolStripMenuItem.Click += new System.EventHandler( this.trimEverythingToolStripMenuItem_Click );
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "xml";
			this.saveFileDialog.Filter = "XML files|*.xml|All files|*.*";
			this.saveFileDialog.RestoreDirectory = true;
			this.saveFileDialog.SupportMultiDottedExtensions = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.HideButtonSize = 8;
			this.splitContainer1.Location = new System.Drawing.Point( 0, 24 );
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add( this.treeView );
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add( this.propertyGrid );
			this.splitContainer1.PrimaryPanel = System.Windows.Forms.FixedPanel.None;
			this.splitContainer1.Size = new System.Drawing.Size( 902, 793 );
			this.splitContainer1.SplitterDistance = 609;
			this.splitContainer1.TabIndex = 0;
			// 
			// propertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Location = new System.Drawing.Point( 8, 0 );
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size( 279, 791 );
			this.propertyGrid.TabIndex = 1;
			// 
			// ContextDebugForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 902, 839 );
			this.Controls.Add( this.splitContainer1 );
			this.Controls.Add( this.statusStrip );
			this.Controls.Add( this.menuStrip );
			this.MainMenuStrip = this.menuStrip;
			this.Name = "ContextDebugForm";
			this.Text = "ContextDebugForm";
			this.statusStrip.ResumeLayout( false );
			this.statusStrip.PerformLayout();
			this.menuStrip.ResumeLayout( false );
			this.menuStrip.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout( false );
			this.splitContainer1.Panel2.ResumeLayout( false );
			this.splitContainer1.ResumeLayout( false );
			this.ResumeLayout( false );
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripProgressBar progress;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem nodeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem printToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem flattenPeersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem expandSuccessToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rebuildToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem trimNodeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem trimEverythingToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private zeroflag.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.PropertyGrid propertyGrid;
	}
}