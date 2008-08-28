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
		protected override void Dispose( bool disposing )
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( Form1 ) );
			this.testTree1 = new Test.TestTree();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// testTree1
			// 
			this.testTree1.AutoSynchronize = false;
			this.testTree1.AutoSynchronizeInterval = 2000;
			this.testTree1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.testTree1.CheckBoxes = false;
			this.testTree1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.testTree1.DrawMode = System.Windows.Forms.TreeViewDrawMode.Normal;
			this.testTree1.FullRowSelect = false;
			this.testTree1.HideSelection = true;
			this.testTree1.HotTracking = false;
			this.testTree1.ImageIndex = -1;
			this.testTree1.ImageKey = "";
			this.testTree1.ImageList = null;
			this.testTree1.Indent = 19;
			this.testTree1.ItemHeight = 16;
			this.testTree1.LabelEdit = false;
			this.testTree1.LineColor = System.Drawing.Color.Black;
			this.testTree1.Location = new System.Drawing.Point( 0, 0 );
			this.testTree1.Name = "testTree1";
			this.testTree1.Node = null;
			this.testTree1.PathSeparator = "\\";
			this.testTree1.RightToLeftLayout = false;
			this.testTree1.Scrollable = true;
			this.testTree1.SelectedImageIndex = -1;
			this.testTree1.SelectedImageKey = "";
			this.testTree1.SelectedItem = null;
			this.testTree1.SelectedNode = null;
			this.testTree1.ShowLines = true;
			this.testTree1.ShowNodeToolTips = false;
			this.testTree1.ShowPlusMinus = true;
			this.testTree1.ShowRootLines = true;
			this.testTree1.Size = new System.Drawing.Size( 357, 273 );
			this.testTree1.Sorted = false;
			this.testTree1.StateImageList = null;
			this.testTree1.TabIndex = 0;
			this.testTree1.TopNode = null;
			this.testTree1.TreeViewNodeSorter = null;
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Right;
			this.propertyGrid1.Location = new System.Drawing.Point( 357, 0 );
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size( 211, 273 );
			this.propertyGrid1.TabIndex = 1;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size( 568, 273 );
			this.Controls.Add( this.testTree1 );
			this.Controls.Add( this.propertyGrid1 );
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout( false );

		}

		#endregion

		private TestTree testTree1;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
	}
}