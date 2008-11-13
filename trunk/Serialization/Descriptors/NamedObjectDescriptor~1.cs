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


		#region FallbackDescriptor
		private Descriptor _FallbackDescriptor;

		/// <summary>
		/// A descriptor used as a fallback if this descriptor cannot resolve a suitable isntance. (ObjectDescriptor by default)
		/// </summary>
		public Descriptor FallbackDescriptor
		{
			get { return _FallbackDescriptor ?? ( _FallbackDescriptor = this.FallbackDescriptorCreate ); }
			set { _FallbackDescriptor = value; }
		}

		/// <summary>
		/// Creates the default/initial value for FallbackDescriptor.
		/// A descriptor used as a fallback if this descriptor cannot resolve a suitable isntance. (ObjectDescriptor by default)
		/// </summary>
		protected virtual Descriptor FallbackDescriptorCreate
		{
			get
			{
				var fallbackDescriptor = _FallbackDescriptor = new ObjectDescriptor();

				return fallbackDescriptor;
			}
		}

		#endregion FallbackDescriptor


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
			object value = null;
			try
			{
				value = this.Provider.ObjectCreationHandler( this.Inner.Find( inn => this.Provider.NameProperty == inn.Name ).Value + "" );
			}
			catch { value = null; }
			if ( value == null )
			{
				this.FallbackDescriptor.Name = this.Name;
				this.FallbackDescriptor.Type = this.Type;
				this.FallbackDescriptor.Value = this.Value;
				this.FallbackDescriptor.Parsed = this.Parsed;
				this.FallbackDescriptor.Context = this.Context;
				this.FallbackDescriptor.Id = this.Id;
				this.FallbackDescriptor.Inner.AddRange( this.Inner );
				this.FallbackDescriptor.IsNull = this.IsNull;
				this.FallbackDescriptor.IsReferenced = this.IsReferenced;
				this.FallbackDescriptor.Parsed = this.Parsed;
				//if ( this.FallbackDescriptor.Parsed = this.Parsed )
				//this.FallbackDescriptor.Parse();
				value = this.FallbackDescriptor.GenerateLink();
			}
			return value;
		}
	}
}
