using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zeroflag.Forms.Reflected
{
	public partial class TypeDescriptionEditorControl : UserControl
	{
		public TypeDescriptionEditorControl()
		{
			InitializeComponent();
		}

		#region Value

		private TypeDescription _Value;

		public TypeDescription Value
		{
			get { return _Value; }
			set
			{
				if ( _Value != value )
				{
					_Value = value;

					this.Synchronize();
				}
			}
		}

		public void Synchronize()
		{
			this.list.Items.Clear();
			if ( this.Value != null )
			{
				foreach ( var prop in this.Value.Properties )
				{
					this.list.Items.Add( prop, prop.Visible );
				}
			}
		}

		#endregion Value

		System.Windows.Forms.Design.IWindowsFormsEditorService EditorService;

		public void Start( System.Windows.Forms.Design.IWindowsFormsEditorService edSvc, object value )
		{
			this.EditorService = edSvc;
			this.Value = (TypeDescription)value;
		}

		public void End()
		{
			this.EditorService = null;
			this.Value = null;
		}

		private void list_ItemCheck( object sender, ItemCheckEventArgs e )
		{
			if ( this.Value != null )
			{
				this.Value.Properties[ e.Index ].Visible = e.NewValue == CheckState.Checked;
				//MessageBox.Show( "Changed " + this.Value.Properties[ e.Index ] + " from " + e.CurrentValue + " to " + e.NewValue );
			}
		}
	}
}
