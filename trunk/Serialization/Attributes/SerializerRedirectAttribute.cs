using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization
{
	/// <summary>
	/// Redirect the serialization of a property to another property.
	/// </summary>
	public class SerializerRedirectAttribute : zeroflag.Serialization.Attributes.Attribute
	{
		#region Target
		private string _Target;

		/// <summary>
		/// The target property for redirection.
		/// </summary>
		public string Target
		{
			get { return _Target; }
			set
			{
				if ( _Target != value )
				{
					_Target = value;
				}
			}
		}

		#endregion Target

		public SerializerRedirectAttribute()
			: this( null )
		{
		}
		public SerializerRedirectAttribute( string target )
		{
			this.Target = target;
		}

	}
}
