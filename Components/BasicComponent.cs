using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Components
{
	public abstract class BasicComponent : System.ComponentModel.IComponent, IBasicComponent
	{
		
		#region IsDisposed

		private bool _IsDisposed;

		public virtual bool IsDisposed
		{
			get { return _IsDisposed; }
			protected set
			{
				if ( _IsDisposed != value )
				{
					_IsDisposed = value;
				}
			}
		}
		#endregion IsDisposed

		#region System.IDisposable

		public virtual void Dispose()
		{
			this.Component.Dispose();
		}

		#endregion System.IDisposable

		private System.ComponentModel.Component Component = new System.ComponentModel.Component();

		protected System.ComponentModel.Component Component
		{
			get { return Component; }
			set { Component = value; }
		}


		#region System.ComponentModel.IComponent

		public virtual System.ComponentModel.ISite Site
		{
			get { return this.Component.Site; }
			set { this.Component.Site = value; }
		}

		#endregion System.ComponentModel.IComponent


		#region System.ComponentModel.Component

		public System.ComponentModel.IContainer Container
		{
			get { return this.Component.Container; }
		}

		public override System.String ToString()
		{
			return this.Component.ToString();
		}

		public virtual System.Object GetLifetimeService()
		{
			return this.Component.GetLifetimeService();
		}

		public virtual System.Object InitializeLifetimeService()
		{
			return this.Component.InitializeLifetimeService();
		}

		public virtual System.Runtime.Remoting.ObjRef CreateObjRef( System.Type requestedType )
		{
			return this.Component.CreateObjRef( requestedType );
		}

		public override bool Equals( System.Object obj )
		{
			return this.Component.Equals( obj );
		}

		public override int GetHashCode()
		{
			return this.Component.GetHashCode();
		}

		#endregion System.ComponentModel.Component
	}
}
