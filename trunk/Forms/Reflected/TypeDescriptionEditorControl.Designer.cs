namespace zeroflag.Forms.Reflected
{
	partial class TypeDescriptionEditorControl
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
			this.list = new System.Windows.Forms.CheckedListBox();
			this.SuspendLayout();
			// 
			// list
			// 
			this.list.Dock = System.Windows.Forms.DockStyle.Fill;
			this.list.FormattingEnabled = true;
			this.list.Location = new System.Drawing.Point( 0, 0 );
			this.list.Name = "list";
			this.list.Size = new System.Drawing.Size( 200, 199 );
			this.list.TabIndex = 0;
			this.list.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler( this.list_ItemCheck );
			// 
			// TypeDescriptionEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this.list );
			this.Name = "TypeDescriptionEditorControl";
			this.Size = new System.Drawing.Size( 200, 200 );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.CheckedListBox list;
	}
}
