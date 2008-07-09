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
			this.treeView = new TreeView<T>();
			this.contextMenu = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.contextAdd = new System.Windows.Forms.ToolStripMenuItem();
			this.loadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// taskProcessor
			// 
			this.taskProcessor.Cancel = false;
			// 
			// treeView1
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.Location = new System.Drawing.Point( 0, 0 );
			this.treeView.Name = "treeView1";
			this.treeView.Size = new System.Drawing.Size( 300, 300 );
			this.treeView.TabIndex = 0;
			// 
			// contextMenu
			// 
			this.contextMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.contextAdd} );
			this.contextMenu.Name = "contextMenu";
			this.contextMenu.Size = new System.Drawing.Size( 153, 48 );
			// 
			// contextAdd
			// 
			this.contextAdd.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.loadingToolStripMenuItem} );
			this.contextAdd.Name = "contextAdd";
			this.contextAdd.Size = new System.Drawing.Size( 152, 22 );
			this.contextAdd.Text = "Add";
			this.contextAdd.DropDownOpening += new System.EventHandler( this.contextAdd_DropDownOpening );
			// 
			// loadingToolStripMenuItem
			// 
			this.loadingToolStripMenuItem.Name = "loadingToolStripMenuItem";
			this.loadingToolStripMenuItem.Size = new System.Drawing.Size( 152, 22 );
			this.loadingToolStripMenuItem.Text = "loading...";
			// 
			// TreeEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.treeView );
			this.Name = "TreeEditor";
			this.Size = new System.Drawing.Size( 300, 300 );
			this.contextMenu.ResumeLayout( false );
			this.ResumeLayout( false );

		}

		#endregion

		private TaskProcessor taskProcessor;
		private zeroflag.Forms.Reflected.TreeView<T> treeView;
		private System.Windows.Forms.ContextMenuStrip contextMenu;
		private System.Windows.Forms.ToolStripMenuItem contextAdd;
		private System.Windows.Forms.ToolStripMenuItem loadingToolStripMenuItem;
	}
}
