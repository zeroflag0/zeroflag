using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public class NamedObjectDescriptor<T> : Descriptor<T>
		where T : class
	{
		public NamedObjectDescriptor()
		{
			try
			{
				this.Inner.Add( this.NameDescriptor );// { Context = this.Context, Name = this.Provider.NameProperty } );
			}
			catch ( Exception exc )
			{
				Console.WriteLine( exc );
			}
		}

		#region NameDescriptor
		private StringDescriptor _NameDescriptor;

		/// <summary>
		/// The descriptor used to describe the object's name.
		/// </summary>
		public StringDescriptor NameDescriptor
		{
			get { return _NameDescriptor ?? ( _NameDescriptor = this.NameDescriptorCreate ); }
			//protected set
			//{
			//	if (_NameDescriptor != value)
			//	{
			//		//if (_NameDescriptor != null) { }
			//		_NameDescriptor = value;
			//		//if (_NameDescriptor != null) { }
			//	}
			//}
		}

		/// <summary>
		/// Creates the default/initial value for NameDescriptor.
		/// The descriptor used to describe the object's name.
		/// </summary>
		protected virtual StringDescriptor NameDescriptorCreate
		{
			get
			{
				var value = _NameDescriptor = new StringDescriptor() { Context = this.Context };
				return value;
			}
		}

		#endregion NameDescriptor


		#region Provider
		private NamedObjectProvider<T> _Provider;

		/// <summary>
		/// The provider containing the named objects for this descriptor.
		/// </summary>
		public NamedObjectProvider<T> Provider
		{
			get { return _Provider ?? ( _Provider = this.ProviderCreate ); }
			protected set { _Provider = value; }
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

		bool ranCustomization = false;
		protected override void OnContextChanged( Context oldvalue, Context newvalue )
		{
			if ( oldvalue != null )
			{
				oldvalue.NamedObjects.ItemRemoved -= NamedObjects_ItemRemoved;
			}
			if ( newvalue != null )
			{
				newvalue.NamedObjects.ItemRemoved += NamedObjects_ItemRemoved;
				this.Provider = null;
				if ( !ranCustomization && this.Provider.DescriptorCustomizationHandle != null )
				{
					this.Provider.DescriptorCustomizationHandle( this );
					ranCustomization = true;
				}
				foreach ( var desc in this.Inner )
				{
					desc.Context = this.Context;
					//desc.Name = this.Provider.NameProperty;
				}
				this.NameDescriptor.Name = this.Provider.NameProperty;
				this.NameDescriptor.Property = this.Provider.GetType().GetProperty( "NameProperty" );
			}
			base.OnContextChanged( oldvalue, newvalue );
		}

		void NamedObjects_ItemRemoved( NamedObjectProvider item )
		{
			if ( item.Type == typeof( T ) )
				this._Provider = null;
		}

		public override System.Reflection.PropertyInfo FindProperty( string property, bool byType )
		{
			Descriptor desc;
			if ( !byType )
			{
				desc = this.Inner.Find( d => d.Name == property ) ?? this.Inner.Find( d => d.Name.ToLower() == property.ToLower() );
			}
			else
			{
				desc = this.Inner.Find( d => d.Type.Name == property ) ?? this.Inner.Find( d => d.Type.Name.ToLower() == property.ToLower() );
			}
			if ( desc != null )
				return desc.Property;
			else
				return null;
			//if ( !byType )
			//{
			//    if ( property == this.Provider.NameProperty )
			//        return this.Provider.GetType().GetProperty( "NameProperty" );
			//}
			//return base.FindProperty( property, byType );
		}

		public override void Parse()
		{
			T value = this.GetValue();
			try
			{
				if ( !ranCustomization && this.Provider.DescriptorCustomizationHandle != null )
				{
					this.Provider.DescriptorCustomizationHandle( this );
					ranCustomization = true;
				}
				if ( this.Property == null || this.Name == null )
				{
					this.Name = this.Provider.TypeNameProvider( value );
				}
				//this.Inner.Add( new StringDescriptor() { Context = this.Context, Name = this.Provider.NameProperty, Value = this.Provider.NameProvider( value ) } );
				foreach ( var desc in this.Inner )
				{
					desc.Context = this.Context;
					if ( desc.Name == this.Provider.NameProperty || desc == this.NameDescriptor )
						desc.Value = this.Provider.NameProvider( value );
					else
					{
						if ( desc.Property != null )
						{
							if ( desc.Value == null )
							{
								desc.Value = desc.Property.GetValue( value, null );
							}
							if ( desc.Name == null )
							{
								desc.Name = desc.Property.Name;
							}
						}
						desc.Parse();
					}
				}
			}
			catch
				( Exception exc )
			{
				Console.WriteLine( exc );

				this.FallbackDescriptor.Name = this.Name;
				this.FallbackDescriptor.Type = this.Type;
				this.FallbackDescriptor.Value = this.Value;
				this.FallbackDescriptor.Parsed = this.Parsed;
				this.FallbackDescriptor.Context = this.Context;
				this.FallbackDescriptor.Id = this.Id;
				this.FallbackDescriptor.Inner.AddRange( this.Inner );
				this.FallbackDescriptor.IsNull = this.IsNull;
				this.FallbackDescriptor.IsReferenced = this.IsReferenced;
				this.FallbackDescriptor.Parsed = false;
				this.FallbackDescriptor.Parse();
			}
		}

		public override object GenerateLink()
		{
			object value = null;
			try
			{
				if ( !ranCustomization && this.Provider.DescriptorCustomizationHandle != null )
				{
					this.Provider.DescriptorCustomizationHandle( this );
					ranCustomization = true;
				}

				this.Value = value = this.Provider.ObjectCreationHandler( this.Inner.Find( inn => this.Provider.NameProperty == inn.Name ).Value + "" );

				foreach ( var desc in this.Inner )
				{
					if ( desc.Name == this.Provider.NameProperty || desc == this.NameDescriptor )
						continue;
					if ( desc.Property == null && desc.Name != null )
					{
						desc.Property = typeof( T ).GetProperty( desc.Name );
					}
					if ( desc.Property != null )
					{
						if ( desc.Value == null )
						{
							desc.Value = desc.Property.GetValue( value, null );
						}
						if ( desc.Name == null )
						{
							desc.Name = desc.Property.Name;
						}
					}

					desc.GenerateParse();
					desc.GenerateCreate();
					//desc.Parse();
					var result = desc.GenerateLink();
					if ( desc.Property != null && desc.Property.CanWrite )
					{
						desc.Property.SetValue( desc.Value, result, null );
					}
				}
			}
			catch ( Exception exc )
			{
				Console.WriteLine( exc );
				value = null;
			}
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
