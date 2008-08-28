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

		#region CyclicReference

		private bool _CyclicReference;

		/// <summary>
		/// Whether the value creates a cyclic reference with a parent-object.
		/// </summary>
		public bool CyclicReference
		{
			get { return _CyclicReference; }
			set
			{
				if ( _CyclicReference != value )
				{
					_CyclicReference = value;
				}
			}
		}

		#endregion CyclicReference


		#region Value

		private T _Value;

		/// <summary>
		/// The node's value.
		/// </summary>
		public virtual T Value
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

		#region Owner

		private TreeView<T> _Owner;

		/// <summary>
		/// This item's owner TreeView~T.
		/// </summary>
		public TreeView<T> Owner
		{
			get { return _Owner; }
			set
			{
				if ( _Owner != value )
				{
					this.OnOwnerChanged( _Owner, _Owner = value );
				}
			}
		}

		#region OwnerChanged event
		public delegate void OwnerChangedHandler( object sender, TreeView<T> oldvalue, TreeView<T> newvalue );

		private event OwnerChangedHandler _OwnerChanged;
		/// <summary>
		/// Occurs when Owner changes.
		/// </summary>
		public event OwnerChangedHandler OwnerChanged
		{
			add { this._OwnerChanged += value; }
			remove { this._OwnerChanged -= value; }
		}

		/// <summary>
		/// Raises the OwnerChanged event.
		/// </summary>
		protected virtual void OnOwnerChanged( TreeView<T> oldvalue, TreeView<T> newvalue )
		{
			// if there are event subscribers...
			if ( this._OwnerChanged != null )
			{
				// call them...
				this._OwnerChanged( this, oldvalue, newvalue );
			}
		}
		#endregion OwnerChanged event
		#endregion Owner


		public virtual void Add( T child )
		{
			if ( this.Owner != null )
				this.TreeView.Invoke( new Action( () =>
				{
					this.Owner.AddChildCallback( this.Value, child );
				} ) );
		}

		public virtual void Remove( T child )
		{
			if ( this.Owner != null )
				this.TreeView.Invoke( new Action( () =>
				{
					this.Owner.RemoveChildCallback( this.Value, child );
				} ) );
		}

		public virtual void Update()
		{
			if ( this.TreeView != null )
				this.TreeView.Invoke( new Action( () =>
						{
								this.Text = this.Owner.Decorate( this.Value ) + ( this.CyclicReference ? " [CYCLIC REFERENCE]" : "" );
						} ) );
		}
	}
}
