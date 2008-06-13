namespace zeroflag.Forms
{
	partial class SplitContainer
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
			this.buttonHide1 = new System.Windows.Forms.Button();
			this.buttonHide2 = new System.Windows.Forms.Button();
			this.Implementation.Panel1.SuspendLayout();
			this.Implementation.Panel2.SuspendLayout();
			this.Implementation.SuspendLayout();
			this.SuspendLayout();
			// 
			// Implementation
			// 
			this.Implementation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Implementation.Location = new System.Drawing.Point( 0, 0 );
			this.Implementation.Name = "Implementation";
			// 
			// Implementation.Panel1
			// 
			this.Implementation.Panel1.Controls.Add( this.buttonHide1 );
			this.Implementation.Panel1.RegionChanged += new System.EventHandler( this._Implementation_StyleChanged );
			this.Implementation.Panel1.ControlAdded += new System.Windows.Forms.ControlEventHandler( this._Implementation_ControlAdded );
			this.Implementation.Panel1.StyleChanged += new System.EventHandler( this._Implementation_StyleChanged );
			this.Implementation.Panel1.SizeChanged += new System.EventHandler( this._Implementation_StyleChanged );
			// 
			// Implementation.Panel2
			// 
			this.Implementation.Panel2.Controls.Add( this.buttonHide2 );
			this.Implementation.Panel2.RegionChanged += new System.EventHandler( this._Implementation_StyleChanged );
			this.Implementation.Panel2.ControlAdded += new System.Windows.Forms.ControlEventHandler( this._Implementation_ControlAdded );
			this.Implementation.Panel2.StyleChanged += new System.EventHandler( this._Implementation_StyleChanged );
			this.Implementation.Panel2.SizeChanged += new System.EventHandler( this._Implementation_StyleChanged );
			this.Implementation.Size = new System.Drawing.Size( 300, 300 );
			this.Implementation.SplitterDistance = 148;
			this.Implementation.TabIndex = 0;
			this.Implementation.StyleChanged += new System.EventHandler( this._Implementation_StyleChanged );
			// 
			// buttonHide1
			// 
			this.buttonHide1.Dock = System.Windows.Forms.DockStyle.Right;
			this.buttonHide1.FlatAppearance.BorderSize = 0;
			this.buttonHide1.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.buttonHide1.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
			this.buttonHide1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonHide1.Location = new System.Drawing.Point( 132, 0 );
			this.buttonHide1.Name = "buttonHide1";
			this.buttonHide1.Size = new System.Drawing.Size( 16, 300 );
			this.buttonHide1.TabIndex = 0;
			this.buttonHide1.TabStop = false;
			this.buttonHide1.Text = ">";
			this.buttonHide1.UseVisualStyleBackColor = false;
			this.buttonHide1.Click += new System.EventHandler( this.buttonHide1_Click );
			// 
			// buttonHide2
			// 
			this.buttonHide2.Dock = System.Windows.Forms.DockStyle.Left;
			this.buttonHide2.FlatAppearance.BorderSize = 0;
			this.buttonHide2.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.buttonHide2.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.Highlight;
			this.buttonHide2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonHide2.Location = new System.Drawing.Point( 0, 0 );
			this.buttonHide2.Name = "buttonHide2";
			this.buttonHide2.Size = new System.Drawing.Size( 16, 300 );
			this.buttonHide2.TabIndex = 0;
			this.buttonHide2.TabStop = false;
			this.buttonHide2.Text = "<";
			this.buttonHide2.UseVisualStyleBackColor = false;
			this.buttonHide2.Click += new System.EventHandler( this.buttonHide2_Click );
			// 
			// SplitContainer
			// 
			this.Name = "SplitContainer";
			this.Size = new System.Drawing.Size( 300, 300 );
			this.Implementation.Panel1.ResumeLayout( false );
			this.Implementation.Panel2.ResumeLayout( false );
			this.Implementation.ResumeLayout( false );
			this.ResumeLayout( false );

		}

		#endregion

		//private System.Windows.Forms.SplitContainer Implementation;
		private System.Windows.Forms.Button buttonHide2;
		private System.Windows.Forms.Button buttonHide1;

	}
}
