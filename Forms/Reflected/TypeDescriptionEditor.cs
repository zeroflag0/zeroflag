using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;

namespace zeroflag.Forms.Reflected
{
	public class TypeDescriptionEditor : System.Drawing.Design.UITypeEditor
	{
		public TypeDescriptionEditor()
		{
		}

		private TypeDescriptionEditorControl typeDescriptionEditorControl;

		public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value )
		{
			if ( provider != null )
			{
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService( typeof( IWindowsFormsEditorService ) );
				if ( edSvc == null )
				{
					return value;
				}
				if ( this.typeDescriptionEditorControl == null )
				{
					this.typeDescriptionEditorControl = new TypeDescriptionEditorControl();
				}
				this.typeDescriptionEditorControl.Start( edSvc, value );
				edSvc.DropDownControl( this.typeDescriptionEditorControl );
				if ( this.typeDescriptionEditorControl.Value != null )
				{
					value = this.typeDescriptionEditorControl.Value;
				}
				this.typeDescriptionEditorControl.End();
			}
			return value;
		}

		public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
		{
			return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
		}}
}
