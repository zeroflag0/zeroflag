using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Forms.Reflected
{
	public class PropertyTreeViewItem : TreeViewItem<object>
	{
		protected override void OnValueChanged( object oldvalue, object newvalue )
		{
			if ( newvalue is PropertyTreeView.PropertyDescriptor )
			{
				this.Descriptor = ( newvalue as PropertyTreeView.PropertyDescriptor );
				this.Value = this.Descriptor.Value;
			}
			else
				base.OnValueChanged( oldvalue, newvalue );
		}

		public override object Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				if ( value is PropertyTreeView.PropertyDescriptor )
				{
					this.Descriptor = ( value as PropertyTreeView.PropertyDescriptor );
					base.Value = this.Descriptor.Value;
				}
				else
					base.Value = value;
			}
		}

		public override void Update()
		{
			string name = "";
			var desc = this.Descriptor;
			if ( desc != null )
			{
				name = desc.Property.Name + ": ";
			}
			if ( this.TreeView != null )
				this.TreeView.Invoke( new Action( () =>
				{

					this.Text = name + this.Owner.Decorate( this.Value ) + ( this.CyclicReference ? " [CYCLIC REFERENCE]" : "" );
				} ) );
		}

		#region Descriptor

		private PropertyTreeView.PropertyDescriptor _Descriptor;

		/// <summary>
		/// Property descriptor.
		/// </summary>
		public PropertyTreeView.PropertyDescriptor Descriptor
		{
			get { return _Descriptor; }
			set
			{
				if ( _Descriptor != value )
				{
					_Descriptor = value;
				}
			}
		}

		#endregion Descriptor

	}
}
