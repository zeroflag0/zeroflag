using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace zeroflag.Forms.Reflected
{
	public class TreeViewItem<T> : System.Windows.Forms.TreeNode
		where T : class
	{
		public TreeViewItem()
			: base()
		{
		}

		public TreeViewItem( T value )
			: this()
		{
			this.Value = value;
		}

		#region Value

		private T _Value;

		/// <summary>
		/// The node's value.
		/// </summary>
		public T Value
		{
			get { return _Value; }
			set
			{
				if ( _Value != value )
				{
					this.OnValueChanged( _Value, _Value = value );
				}
			}
		}

		#region ValueChanged event
		public delegate void ValueChangedHandler( object sender, T oldvalue, T newvalue );

		private event ValueChangedHandler _ValueChanged;
		/// <summary>
		/// Occurs when Value changes.
		/// </summary>
		public event ValueChangedHandler ValueChanged
		{
			add { this._ValueChanged += value; }
			remove { this._ValueChanged -= value; }
		}

		/// <summary>
		/// Raises the ValueChanged event.
		/// </summary>
		protected virtual void OnValueChanged( T oldvalue, T newvalue )
		{
			// if there are event subscribers...
			if ( this._ValueChanged != null )
			{
				// call them...
				this._ValueChanged( this, oldvalue, newvalue );
			}
			this.Update();
		}
		#endregion ValueChanged event
		#endregion Value

		public void Update()
		{
			this.TreeView.Invoke( new Action( () =>
					{
						this.Text = ( (object)this.Value ?? "<null>" ).ToString();
					} ) );
		}
	}
}
