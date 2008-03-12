namespace zeroflag.Forms.Reflected
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
			this._List = new System.Windows.Forms.ListView();
			this.SuspendLayout();
			// 
			// List
			// 
			this._List.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._List.Dock = System.Windows.Forms.DockStyle.Fill;
			this._List.FullRowSelect = true;
			this._List.Location = new System.Drawing.Point(0, 0);
			this._List.Name = "_List";
			this._List.Size = new System.Drawing.Size(150, 150);
			this._List.TabIndex = 0;
			this._List.UseCompatibleStateImageBehavior = false;
			this._List.View = System.Windows.Forms.View.Details;
			// 
			// ListView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._List);
			this.Name = "ListView";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView _List;
	}
}
