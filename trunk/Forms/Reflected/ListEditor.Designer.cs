namespace zeroflag.Forms.Reflected
{
	partial class ListEditor<T>
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
			this.listView = new ListView<T>();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.contextAdd = new System.Windows.Forms.ContextMenuStrip( this.components );
			this.loadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.panel1.SuspendLayout();
			this.contextAdd.SuspendLayout();
			this.SuspendLayout();
			// 
			// ListView
			// 
			this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView.Location = new System.Drawing.Point( 0, 0 );
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size( 150, 276 );
			this.listView.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.Controls.Add( this.buttonAdd );
			this.panel1.Controls.Add( this.buttonRemove );
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point( 0, 276 );
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size( 150, 24 );
			this.panel1.TabIndex = 1;
			// 
			// buttonRemove
			// 
			this.buttonRemove.Dock = System.Windows.Forms.DockStyle.Left;
			this.buttonRemove.Location = new System.Drawing.Point( 0, 0 );
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size( 75, 24 );
			this.buttonRemove.TabIndex = 2;
			this.buttonRemove.Text = "Remove";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new System.EventHandler( this.buttonRemove_Click );
			// 
			// buttonAdd
			// 
			this.buttonAdd.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonAdd.Location = new System.Drawing.Point( 75, 0 );
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size( 75, 24 );
			this.buttonAdd.TabIndex = 3;
			this.buttonAdd.Text = "Add";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler( this.buttonAdd_Click );
			// 
			// contextAdd
			// 
			this.contextAdd.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.loadingToolStripMenuItem} );
			this.contextAdd.Name = "contextAdd";
			this.contextAdd.Size = new System.Drawing.Size( 131, 26 );
			// 
			// loadingToolStripMenuItem
			// 
			this.loadingToolStripMenuItem.Enabled = false;
			this.loadingToolStripMenuItem.Name = "loadingToolStripMenuItem";
			this.loadingToolStripMenuItem.Size = new System.Drawing.Size( 130, 22 );
			this.loadingToolStripMenuItem.Text = "loading...";
			// 
			// ListEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.listView );
			this.Controls.Add( this.panel1 );
			this.Name = "ListEditor";
			this.Size = new System.Drawing.Size( 150, 300 );
			this.panel1.ResumeLayout( false );
			this.contextAdd.ResumeLayout( false );
			this.ResumeLayout( false );

		}

		#endregion

		private ListView<T> listView;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.Button buttonAdd;
		private System.Windows.Forms.ContextMenuStrip contextAdd;
		private System.Windows.Forms.ToolStripMenuItem loadingToolStripMenuItem;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
	}
}
