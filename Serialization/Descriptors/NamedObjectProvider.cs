using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public abstract class NamedObjectProvider
	{
		#region Context

		private Context _Context;

		/// <summary>
		/// This provider's context
		/// </summary>
		public Context Context
		{
			get { return _Context; }
			set
			{
				if ( _Context != value )
				{
					this.OnContextChanged( _Context, _Context = value );
				}
			}
		}

		#region ContextChanged event
		public delegate void ContextChangedHandler( object sender, Context oldvalue, Context newvalue );

		private event ContextChangedHandler _ContextChanged;
		/// <summary>
		/// Occurs when Context changes.
		/// </summary>
		public event ContextChangedHandler ContextChanged
		{
			add { this._ContextChanged += value; }
			remove { this._ContextChanged -= value; }
		}

		/// <summary>
		/// Raises the ContextChanged event.
		/// </summary>
		protected virtual void OnContextChanged( Context oldvalue, Context newvalue )
		{
			// if there are event subscribers...
			if ( this._ContextChanged != null )
			{
				// call them...
				this._ContextChanged( this, oldvalue, newvalue );
			}
		}
		#endregion ContextChanged event
		#endregion Context


		#region Type
		private Type _Type;

		/// <summary>
		/// The type of objects provided.
		/// </summary>
		public Type Type
		{
			get { return _Type ?? ( _Type = this.TypeCreate ); }
			//set { _Type = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Type.
		/// The type of objects provided.
		/// </summary>
		protected abstract Type TypeCreate
		{
			get;
		}

		#endregion Type

	}
}
