namespace zeroflag.Forms.Reflected
{
	partial class TreeEditor<T>
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.taskProcessor = new zeroflag.Forms.TaskProcessor( this.components );
			//this = new zeroflag.Forms.Reflected.TreeView<T>();
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.contextAdd = new System.Windows.Forms.ToolStripMenuItem();
			this.contextRemove = new System.Windows.Forms.ToolStripMenuItem();
			this.contextRemoveConfirm = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.loadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// taskProcessor
			// 
			this.taskProcessor.Cancel = false;
			// 
			// treeView
			// 
			this.ContextMenuStrip = this.contextMenu;
			this.DrawMode = System.Windows.Forms.TreeViewDrawMode.Normal;
			this.FullRowSelect = true;
			this.HideSelection = false;
			this.HotTracking = true;
			this.Name = "treeView";
			this.PathSeparator = ".";
			this.Size = new System.Drawing.Size( 300, 300 );
			this.TabIndex = 0;
			this.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler( treeView_NodeMouseClick );
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.contextAdd, this.contextRemove} );
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size( 97, 26 );
			this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler( contextMenu_Opening );
			// 
			// contextAdd
			// 
			this.contextAdd.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.loadingToolStripMenuItem} );
			this.contextAdd.Name = "contextAdd";
			this.contextAdd.Size = new System.Drawing.Size( 96, 22 );
			this.contextAdd.Text = "Add";
			this.contextAdd.DropDownOpening += new System.EventHandler( this.contextAdd_DropDownOpening );
			// 
			// contextRemove
			// 
			this.contextRemove.Name = "contextRemove";
			this.contextRemove.Size = new System.Drawing.Size( 96, 22 );
			this.contextRemove.Text = "Remove";
			this.contextRemove.Click += new System.EventHandler( contextRemove_Click );
			this.contextRemove.DropDownOpening += new System.EventHandler( this.contextAdd_DropDownOpening );
			// 
			// contextMenu
			// 
			this.contextRemoveConfirm.Name = "contextRemoveConfirm";
			this.contextRemoveConfirm.Size = new System.Drawing.Size( 97, 26 );
			// 
			// loadingToolStripMenuItem
			// 
			this.loadingToolStripMenuItem.Name = "loadingToolStripMenuItem";
			this.loadingToolStripMenuItem.Size = new System.Drawing.Size( 123, 22 );
			this.loadingToolStripMenuItem.Text = "loading...";
			// 
			// TreeEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "TreeEditor";
			this.Size = new System.Drawing.Size( 300, 300 );
			this.contextMenu.ResumeLayout( false );
			this.ResumeLayout( false );

		}

		#endregion

		private TaskProcessor taskProcessor;
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem contextAdd;
		private System.Windows.Forms.ToolStripMenuItem loadingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem contextRemove;
		private System.Windows.Forms.ContextMenuStrip contextRemoveConfirm;

	}
}
