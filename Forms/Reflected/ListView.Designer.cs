﻿namespace zeroflag.Forms.Reflected
{
	partial class ListView<T>
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
			this._Control = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// _Control
			// 
			this._Control.Dock = System.Windows.Forms.DockStyle.Fill;
			this._Control.FormattingEnabled = true;
			this._Control.Location = new System.Drawing.Point( 0, 0 );
			this._Control.Name = "_Control";
			this._Control.Size = new System.Drawing.Size( 150, 147 );
			this._Control.TabIndex = 0;
			// 
			// ListView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add( this._Control );
			this.Name = "ListView";
			this.ResumeLayout( false );

		}

		#endregion

		private System.Windows.Forms.ListBox _Control;

	}
}
