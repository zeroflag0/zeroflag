namespace Test
{
	partial class Form1
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
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.testList1 = new Test.TestList();
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Right;
			this.propertyGrid1.Location = new System.Drawing.Point(570, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(339, 442);
			this.propertyGrid1.TabIndex = 5;
			// 
			// testList1
			// 
			this.testList1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.testList1.Location = new System.Drawing.Point(0, 0);
			this.testList1.MultiSelect = true;
			this.testList1.Name = "testList1";
			this.testList1.Size = new System.Drawing.Size(570, 442);
			this.testList1.TabIndex = 4;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(909, 442);
			this.Controls.Add(this.testList1);
			this.Controls.Add(this.propertyGrid1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private TestList testList1;
		private System.Windows.Forms.PropertyGrid propertyGrid1;

	}
}

