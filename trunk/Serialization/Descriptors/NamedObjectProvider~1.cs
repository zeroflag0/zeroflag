using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Serialization.Descriptors
{
	public class NamedObjectProvider<T> : NamedObjectProvider
		where T : class
	{
		#region NameProperty
		private string _NameProperty;

		/// <summary>
		/// The name of the property providing the object's name. Use <see cref="NameProvider"/> if you need a different source for the name.
		/// </summary>
		public string NameProperty
		{
			get { return _NameProperty ?? ( _NameProperty = this.NamePropertyCreate ); }
			set { _NameProperty = value; }
		}

		/// <summary>
		/// Creates the default/initial value for NameProperty.
		/// The name of the property providing the object's name. Use <see cref="NameProvider"/> if you need a different source for the name.
		/// </summary>
		protected virtual string NamePropertyCreate
		{
			get
			{
				var nameProperty = _NameProperty = "Name";
				return nameProperty;
			}
		}

		#endregion NameProperty



		#region NameProvider
		private GetHandler<string, T> _NameProvider;

		/// <summary>
		/// Used to retrieve a name for an object. By default using the <see cref="NameProperty"/> to retrieve the name.
		/// </summary>
		public GetHandler<string, T> NameProvider
		{
			get { return _NameProvider ?? ( _NameProvider = this.NameProviderCreate ); }
			//set { _NameProvider = value; }
		}

		/// <summary>
		/// Creates the default/initial value for NameProvider.
		/// Used to retrieve a name for an object. By default using the <see cref="NameProperty"/> to retrieve the name.
		/// </summary>
		protected virtual GetHandler<string, T> NameProviderCreate
		{
			get
			{
				var nameProvider = _NameProvider = val =>
					(string)typeof( T ).GetProperty( this.NameProperty ).GetValue( val, null );

				return nameProvider;
			}
		}

		#endregion NameProvider

		#region Objects
		private zeroflag.Collections.Collection<T> _Objects;

		/// <summary>
		/// The objects available.
		/// </summary>
		public zeroflag.Collections.Collection<T> Objects
		{
			get { return _Objects ?? ( _Objects = this.ObjectsCreate ); }
			//set { _Objects = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Objects.
		/// The objects available.
		/// </summary>
		protected virtual zeroflag.Collections.Collection<T> ObjectsCreate
		{
			get
			{
				var Objects = _Objects = new zeroflag.Collections.Collection<T>();

				return Objects;
			}
		}

		#endregion Objects


		protected override void OnContextChanged( Context oldvalue, Context newvalue )
		{
			if ( oldvalue != null )
			{
				oldvalue.DescriptorTypes.Remove( this.Type );
			}
			if ( newvalue != null )
			{
				newvalue.DescriptorTypes.Add( this.Type, typeof( NamedObjectDescriptor<> ).Specialize( this.Type ) );
			}
			base.OnContextChanged( oldvalue, newvalue );
		}


		#region ObjectCreationHandler
		private GetHandler<T, string> _ObjectCreationHandler;

		/// <summary>
		/// Handler used to create the objects...
		/// </summary>
		public GetHandler<T, string> ObjectCreationHandler
		{
			get { return _ObjectCreationHandler ?? ( _ObjectCreationHandler = this.ObjectCreationHandlerCreate ); }
			set { _ObjectCreationHandler = value; }
		}

		/// <summary>
		/// Creates the default/initial value for ObjectCreationHandler.
		/// Handler used to create the objects...
		/// </summary>
		protected virtual GetHandler<T, string> ObjectCreationHandlerCreate
		{
			get
			{
				var handler = _ObjectCreationHandler = name => this.Objects.Find( item => this.NameProvider( item ) == name ) ?? this.Objects.Find( item => ( this.NameProvider( item ) + "" ).Trim().ToLower() == ( name + "" ).Trim().ToLower() );

				return handler;
			}
		}

		#endregion ObjectCreationHandler


		protected override Type TypeCreate
		{
			get { return typeof( T ); }
		}

		public static implicit operator NamedObjectProvider<T>( T[] list )
		{
			return new NamedObjectProvider<T>( list );
		}

		public NamedObjectProvider()
		{
		}

		public NamedObjectProvider( IEnumerable<T> list )
			: this()
		{
			this.Objects.AddRange( list );
		}

		public NamedObjectProvider( params T[] list )
			: this( (IEnumerable<T>)list )
		{
		}
	}
}
