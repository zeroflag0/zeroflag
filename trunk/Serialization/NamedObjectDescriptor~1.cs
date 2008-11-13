using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public class NamedObjectDescriptor<T> : Descriptor<T>
		where T : class
	{

		#region Provider
		private NamedObjectProvider<T> _Provider;

		/// <summary>
		/// The provider containing the named objects for this descriptor.
		/// </summary>
		public NamedObjectProvider<T> Provider
		{
			get { return _Provider ?? ( _Provider = this.ProviderCreate ); }
			//set { _Provider = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Provider.
		/// The provider containing the named objects for this descriptor.
		/// </summary>
		protected virtual NamedObjectProvider<T> ProviderCreate
		{
			get
			{
				var provider = _Provider = this.Context.NamedObjects.Find<NamedObjectProvider<T>>();

				return provider;
			}
		}

		#endregion Provider

		protected override void OnContextChanged( Context oldvalue, Context newvalue )
		{
			if ( oldvalue != null )
			{
				oldvalue.NamedObjects.ItemRemoved -= NamedObjects_ItemRemoved;
			}
			if ( newvalue != null )
			{
				newvalue.NamedObjects.ItemRemoved += NamedObjects_ItemRemoved;

			}
			base.OnContextChanged( oldvalue, newvalue );
		}

		void NamedObjects_ItemRemoved( NamedObjectProvider item )
		{
			if ( item.Type == typeof( T ) )
				this._Provider = null;
		}

		public override void Parse()
		{
		}

		public override object GenerateLink()
		{
			return this.Provider.ObjectCreationHandler( this.Inner.Find( inn => this.Provider.NameProperty == inn.Name ).Value + "" );
		}
	}
}
