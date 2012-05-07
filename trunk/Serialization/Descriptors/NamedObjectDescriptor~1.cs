using System;
using System.Linq;
using System.Reflection;

namespace zeroflag.Serialization.Descriptors
{
	public class NamedObjectDescriptor<T> : Descriptor<T>
		where T : class
	{
		private bool _IsLinked;
		private bool ranCustomization;

		public NamedObjectDescriptor()
		{
			try
			{
				this.Inner.Add(this.NameDescriptor); // { Context = this.Context, Name = this.Provider.NameProperty } );
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
			}
		}

		#region NameDescriptor

		private StringDescriptor _NameDescriptor;

		/// <summary>
		/// The descriptor used to describe the object's name.
		/// </summary>
		public StringDescriptor NameDescriptor
		{
			get { return this._NameDescriptor ?? (this._NameDescriptor = this.NameDescriptorCreate); }
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
				StringDescriptor value = this._NameDescriptor = new StringDescriptor { Context = this.Context };
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
			get { return this._Provider ?? (this._Provider = this.ProviderCreate); }
			protected set { this._Provider = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Provider.
		/// The provider containing the named objects for this descriptor.
		/// </summary>
		protected virtual NamedObjectProvider<T> ProviderCreate
		{
			get
			{
				NamedObjectProvider<T> provider = this._Provider = this.Context.NamedObjects.Find<NamedObjectProvider<T>>();

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
			get { return this._FallbackDescriptor ?? (this._FallbackDescriptor = this.FallbackDescriptorCreate); }
			set { this._FallbackDescriptor = value; }
		}

		/// <summary>
		/// Creates the default/initial value for FallbackDescriptor.
		/// A descriptor used as a fallback if this descriptor cannot resolve a suitable isntance. (ObjectDescriptor by default)
		/// </summary>
		protected virtual Descriptor FallbackDescriptorCreate
		{
			get
			{
				Descriptor fallbackDescriptor = this._FallbackDescriptor = new ObjectDescriptor();

				fallbackDescriptor.Name = this.Name;
				fallbackDescriptor.Type = this.Type;
				fallbackDescriptor.Value = this.Value;
				fallbackDescriptor.Context = this.Context;
				fallbackDescriptor.Id = this.Id;
				fallbackDescriptor.IsNull = this.IsNull;
				fallbackDescriptor.IsReferenced = this.IsReferenced;

				return fallbackDescriptor;
			}
		}

		#endregion FallbackDescriptor

		protected override void OnContextChanged(Context oldvalue, Context newvalue)
		{
			this._IsLinked = false;
			if (oldvalue != null)
			{
				oldvalue.NamedObjects.ItemRemoved -= this.NamedObjects_ItemRemoved;
			}
			if (newvalue != null)
			{
				newvalue.NamedObjects.ItemRemoved += this.NamedObjects_ItemRemoved;
				this.Provider = null;
				if (!this.ranCustomization && this.Provider.DescriptorCustomizationHandle != null)
				{
					this.Provider.DescriptorCustomizationHandle(this);
					this.ranCustomization = true;
				}
				string propname = this.Provider.NameProperty;
				Descriptor propnameinfo = null;
				foreach (Descriptor desc in this.Inner)
				{
					if (desc.Name == propname)
					{
						propnameinfo = desc;
						continue;
					}
					desc.Context = this.Context;
					//desc.Name = this.Provider.NameProperty;
				}
				if (propnameinfo != null)
				{
					this.Inner.Remove(propnameinfo);
				}
				this.NameDescriptor.Name = propname;
				this.NameDescriptor.Property = this.Provider.GetType().GetProperty("NameProperty");
			}
			base.OnContextChanged(oldvalue, newvalue);
		}

		private void NamedObjects_ItemRemoved(NamedObjectProvider item)
		{
			if (item.Type == typeof (T))
			{
				this._Provider = null;
			}
		}

		public override PropertyInfo FindProperty(string property, bool byType)
		{
			//return base.FindProperty( property, byType );

			Descriptor desc;
			if (!byType)
			{
				desc = this.Inner.FirstOrDefault(d => d.Name == property) ?? this.Inner.FirstOrDefault(d => d.Name != null && d.Name.ToLower() == property.ToLower());
			}
			else
			{
				desc = this.Inner.FirstOrDefault(d => d.Type.Name == property) ?? this.Inner.FirstOrDefault(d => d.Name != null && d.Type.Name.ToLower() == property.ToLower());
			}
			if (desc != null)
			{
				return desc.Property;
			}
			else
			{
				return base.FindProperty(property, byType);
			}
			//if ( !byType )
			//{
			//    if ( property == this.Provider.NameProperty )
			//        return this.Provider.GetType().GetProperty( "NameProperty" );
			//}
			//return base.FindProperty( property, byType );
		}

		public override void Parse()
		{
			this._IsLinked = false;
			T value = this.GetValue();
			try
			{
				if (!this.ranCustomization && this.Provider.DescriptorCustomizationHandle != null)
				{
					this.Provider.DescriptorCustomizationHandle(this);
					this.ranCustomization = true;
				}
				if (this.Property == null || this.Name == null)
				{
					this.Name = this.Provider.TypeNameProvider(value);
				}
				//this.Inner.Add( new StringDescriptor() { Context = this.Context, Name = this.Provider.NameProperty, Value = this.Provider.NameProvider( value ) } );

				foreach (Descriptor desc in this.Inner)
				{
					desc.Context = this.Context;
					if (desc.Name == this.Provider.NameProperty || desc == this.NameDescriptor)
					{
						desc.Value = this.Provider.NameProvider(value);
					}
					else
					{
						if (desc.Property == null && desc.Name != null)
						{
							desc.Property = this.Type.GetProperty(desc.Name);
						}
						if (desc.Property != null)
						{
							if (desc.Value == null)
							{
								desc.Value = desc.Property.GetValue(value, null);
							}
							if (desc.Name == null)
							{
								desc.Name = desc.Property.Name;
							}
						}
						this.Context.Parse(desc.Name, desc.Type, desc.Value, desc, this.Value, desc.Property);
						//desc.Parse();
					}
				}
			}
			catch
				(Exception exc)
			{
				Console.WriteLine(exc);

				this.FallbackDescriptor.Name = this.Name;
				this.FallbackDescriptor.Type = this.Type;
				this.FallbackDescriptor.Value = this.Value;
				this.FallbackDescriptor.Parsed = this.Parsed;
				this.FallbackDescriptor.Context = this.Context;
				this.FallbackDescriptor.Id = this.Id;
				this.FallbackDescriptor.Inner.AddRange(this.Inner);
				this.FallbackDescriptor.IsNull = this.IsNull;
				this.FallbackDescriptor.IsReferenced = this.IsReferenced;
				this.FallbackDescriptor.Parsed = false;
				this.FallbackDescriptor.Parse();
			}
		}

		public override object GenerateLink()
		{
			object value = null;
			if (this._IsLinked)
			{
				value = this.Value;
				return value;
			}
			try
			{
				if (!this.ranCustomization && this.Provider.DescriptorCustomizationHandle != null)
				{
					this.Provider.DescriptorCustomizationHandle(this);
					this.ranCustomization = true;
				}

				if (value == null)
				{
					this.Value = value = this.Provider.ObjectCreationHandler(this.Inner.FirstOrDefault(inn => this.Provider.NameProperty == inn.Name).Value + "");
				}

				foreach (Descriptor desc in this.Inner)
				{
					if (desc.Name == this.Provider.NameProperty || desc == this.NameDescriptor)
					{
						continue;
					}
					if (desc.Property == null && desc.Name != null)
					{
						desc.Property = typeof (T).GetProperty(desc.Name);
					}
					if (desc.Property != null)
					{
						if (desc.Value == null)
						{
							desc.Value = desc.Property.GetValue(value, null);
						}
						if (desc.Name == null)
						{
							desc.Name = desc.Property.Name;
						}
					}

					desc.GenerateParse();
					desc.GenerateCreate();
					//desc.Parse();
					object result = desc.GenerateLink();
					if (desc.Property != null && desc.Property.CanWrite)
					{
						desc.Property.SetValue(this.Value, result, null);
					}
				}
				this._IsLinked = true;
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
				value = null;
			}
			if (value == null)
			{
				this.FallbackDescriptor.Name = this.Name;
				this.FallbackDescriptor.Type = this.Type;
				this.FallbackDescriptor.Value = this.Value;
				this.FallbackDescriptor.Parsed = this.Parsed;
				this.FallbackDescriptor.Context = this.Context;
				this.FallbackDescriptor.Id = this.Id;
				this.FallbackDescriptor.Inner.AddRange(this.Inner);
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
