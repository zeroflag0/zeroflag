using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//namespace zeroflag.Serialization.Attributes
//{
	public class SerializerIgnorePropertiesAttribute : zeroflag.Serialization.Attributes.Attribute
	{
		#region PropertyNames
		private string[] _PropertyNames;

		/// <summary>
		/// Names of ignored properties.
		/// </summary>
		public string[] PropertyNames
		{
			get { return _PropertyNames; }
			set
			{
				if ( _PropertyNames != value )
				{
					_PropertyNames = value;
				}
			}
		}

		#endregion PropertyNames

		public SerializerIgnorePropertiesAttribute( params string[] properties )
		{
			this.PropertyNames = properties;
		}
	}
//}
