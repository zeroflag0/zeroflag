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
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("");
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.listView1 = new System.Windows.Forms.ListView();
			this.testList1 = new Test.TestList();
			this.testList2 = new Test.TestList2();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.Items.AddRange(new object[] {
            "asdf",
            "foo"});
			this.listBox1.Location = new System.Drawing.Point(244, 335);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(120, 95);
			this.listBox1.TabIndex = 2;
			// 
			// listView1
			// 
			this.listView1.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1});
			this.listView1.Location = new System.Drawing.Point(403, 333);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(121, 97);
			this.listView1.TabIndex = 3;
			this.listView1.UseCompatibleStateImageBehavior = false;
			// 
			// testList1
			// 
			this.testList1.Dock = System.Windows.Forms.DockStyle.Left;
			this.testList1.Location = new System.Drawing.Point(0, 0);
			this.testList1.Name = "testList1";
			this.testList1.Size = new System.Drawing.Size(221, 442);
			this.testList1.TabIndex = 4;
			// 
			// testList2
			// 
			this.testList2.Location = new System.Drawing.Point(244, 0);
			this.testList2.MyItems.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem2});
			this.testList2.Name = "testList2";
			this.testList2.Size = new System.Drawing.Size(227, 245);
			this.testList2.TabIndex = 1;
			this.testList2.UseCompatibleStateImageBehavior = false;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Right;
			this.propertyGrid1.Location = new System.Drawing.Point(570, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.SelectedObject = this.testList2;
			this.propertyGrid1.Size = new System.Drawing.Size(339, 442);
			this.propertyGrid1.TabIndex = 5;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(909, 442);
			this.Controls.Add(this.propertyGrid1);
			this.Controls.Add(this.testList1);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.listBox1);
			this.Controls.Add(this.testList2);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private TestList2 testList2;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.ListView listView1;
		private TestList testList1;
		private System.Windows.Forms.PropertyGrid propertyGrid1;

	}
}

