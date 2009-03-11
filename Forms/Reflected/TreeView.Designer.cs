namespace zeroflag.Forms.Reflected
{
	partial class TreeView<T>
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this._Control = new System.Windows.Forms.TreeView();
			this.taskProcessor = new zeroflag.Components.TaskProcessor( this.components );
			this.timer = new System.Windows.Forms.Timer( this.components );
			this.SuspendLayout();
			// 
			// _Control
			// 
			this._Control.Dock = System.Windows.Forms.DockStyle.Fill;
			this._Control.Location = new System.Drawing.Point( 0, 0 );
			this._Control.Name = "_Control";
			this._Control.Size = new System.Drawing.Size( 300, 300 );
			this._Control.TabIndex = 0;
			// 
			// taskProcessor
			// 
			this.taskProcessor.Cancel = false;
			// 
			// timer
			// 
			this.timer.Interval = 2000;
			// 
			// TreeView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this._Control );
			this.Name = "TreeView";
			this.Size = new System.Drawing.Size( 300, 300 );
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.TreeView _Control;
		private zeroflag.Components.TaskProcessor taskProcessor;
		private System.Windows.Forms.Timer timer;
	}
}
