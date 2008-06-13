using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace zeroflag.Forms
{
	[System.Runtime.InteropServices.ClassInterface( System.Runtime.InteropServices.ClassInterfaceType.AutoDispatch )]
	[DefaultEvent( "SplitterMoved" )]
	[Docking( DockingBehavior.AutoDock )]
	[Designer( "System.Windows.Forms.Design.SplitContainerDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" )]
	//[System.Runtime.InteropServices.ComVisible( true )]
	public partial class SplitContainer : System.Windows.Forms.SplitContainer
	{
		public SplitContainer()
		{
			InitializeComponent();

			this.Dock = DockStyle.Fill;
		}

		public System.Windows.Forms.SplitContainer Implementation
		{
			get { return this; }
			//set { _Implementation = value; }
		}

		[Browsable( true )]
		[EditorBrowsable( EditorBrowsableState.Always )]
		public new BorderStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle = this.Implementation.BorderStyle = value;
			}
		}


		#region HideButtonSize

		private int _HideButtonSize = 16;

		public virtual int HideButtonSize
		{
			get { return _HideButtonSize; }
			set
			{
				if ( _HideButtonSize != value )
				{
					_HideButtonSize = value;

					this.buttonHide1.Width = this.buttonHide1.Height = this.buttonHide2.Width = this.buttonHide2.Height = value;
					this.Font = this.buttonHide1.Font = this.buttonHide2.Font = new Font( this.buttonHide1.Font.FontFamily, Math.Max( 4f, value - 0f ), GraphicsUnit.Pixel );
				}
			}
		}
		#endregion HideButtonSize


		private void buttonHide1_Click( object sender, EventArgs e )
		{
			if ( this.Panel2Collapsed )
			{
				this.Panel2Collapsed = false;
				this.buttonHide1.Text = "-";
			}
			else
			{
				this.Panel1Collapsed = true;
				this.buttonHide2.Text = "+";
			}
			this.Focus();
			this.Select();
			this.Invalidate( true );
			//this._Implementation_StyleChanged( sender, e );
		}

		private void buttonHide2_Click( object sender, EventArgs e )
		{
			if ( this.Panel1Collapsed )
			{
				this.Panel1Collapsed = false;
				this.buttonHide2.Text = "-";
			}
			else
			{
				this.Panel2Collapsed = true;
				this.buttonHide1.Text = "+";
			}
			this.Focus();
			this.Select();
			this.Invalidate( true );
			//this._Implementation_StyleChanged( sender, e );
		}

		private void _Implementation_StyleChanged( object sender, EventArgs e )
		{
			//MessageBox.Show( this.Orientation.ToString() );
			this.buttonHide2.Text = "-";
			this.buttonHide1.Text = "-";

			if ( this.Panel1Collapsed )
			{
				this.buttonHide2.Text = "+";
			}
			if ( this.Panel2Collapsed )
			{
				this.buttonHide1.Text = "+";
			}

			if ( this.Orientation == Orientation.Horizontal )
			{
				// horizontal

				this.buttonHide1.Dock = DockStyle.Bottom;
				this.buttonHide2.Dock = DockStyle.Top;
				this.buttonHide1.Height = this.buttonHide2.Height = this.HideButtonSize;
			}
			else
			{
				// vertical
				this.buttonHide1.Dock = DockStyle.Right;
				this.buttonHide2.Dock = DockStyle.Left;
				this.buttonHide1.Width = this.buttonHide2.Width = this.HideButtonSize;
			}
			this.buttonHide1.SendToBack();
			this.buttonHide2.SendToBack();
		}

		private void _Implementation_ControlAdded( object sender, ControlEventArgs e )
		{
			this.buttonHide1.SendToBack();
			this.buttonHide2.SendToBack();
		}



	}

}
