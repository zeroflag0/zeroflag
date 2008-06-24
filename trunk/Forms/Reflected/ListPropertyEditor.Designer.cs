namespace zeroflag.Forms.Reflected
{
	partial class ListPropertyEditor<T>
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
			this.splitContainer = new zeroflag.Forms.SplitContainer();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.listView = new ListEditor<T>();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// SplitContainer
			// 
			this.splitContainer.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.HideButtonSize = 8;
			this.splitContainer.Location = new System.Drawing.Point( 0, 0 );
			this.splitContainer.Name = "splitContainer";
			// 
			// SplitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add( this.listView );
			// 
			// SplitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add( this.propertyGrid );
			this.splitContainer.PrimaryPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer.Size = new System.Drawing.Size( 300, 300 );
			this.splitContainer.SplitterDistance = 148;
			this.splitContainer.TabIndex = 0;
			this.splitContainer.TabStop = false;
			// 
			// PropertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Location = new System.Drawing.Point( 8, 0 );
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size( 138, 298 );
			this.propertyGrid.TabIndex = 1;
			// 
			// ListView
			// 
			this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView.Location = new System.Drawing.Point( 0, 0 );
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size( 146, 298 );
			this.listView.TabIndex = 0;
			this.listView.ItemSelected += new System.Action<T>( listView_ItemSelected );
			this.listView.ItemDeselected += new System.Action<T>( listView_ItemDeselected );
			this.listView.ItemAdded += new System.Action<T>( listView_ItemAdded );
			this.listView.ItemRemoved += new System.Action<T>( listView_ItemRemoved );
			// 
			// ListPropertyEditor
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.splitContainer );
			this.Name = "ListPropertyEditor";
			this.Size = new System.Drawing.Size( 300, 300 );
			this.splitContainer.Panel1.ResumeLayout( false );
			this.splitContainer.Panel2.ResumeLayout( false );
			this.splitContainer.ResumeLayout( false );
			this.ResumeLayout( false );

		}

		#endregion

		private SplitContainer splitContainer;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private ListEditor<T> listView;
	}
}
