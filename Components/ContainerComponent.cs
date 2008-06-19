using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Components
{
	public abstract class ContainerComponent : BasicComponent, System.ComponentModel.IContainer
	{
		private System.ComponentModel.Container _Container = new System.ComponentModel.Container();

		protected System.ComponentModel.Container Container
		{
			get { return _Container; }
			set { _Container = value; }
		}


		#region System.IDisposable

		public override void Dispose()
		{
			this._Container.Dispose();
			base.Dispose();
		}

		#endregion System.IDisposable


		#region System.ComponentModel.IContainer

		public virtual System.ComponentModel.ComponentCollection Components
		{
			get { return this._Container.Components; }
		}

		public virtual void Add( System.ComponentModel.IComponent component, string name )
		{
			this._Container.Add( component, name );
		}
		public virtual void Add( System.ComponentModel.IComponent component )
		{
			this._Container.Add( component );
		}

		public virtual void Remove( System.ComponentModel.IComponent component )
		{
			this._Container.Remove( component );
		}

		#endregion System.ComponentModel.IContainer

	}
}
