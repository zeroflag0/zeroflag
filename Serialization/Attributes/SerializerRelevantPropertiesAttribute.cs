using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Serialization.Attributes
{
	public class SerializerRelevantPropertiesAttribute : zeroflag.Serialization.Attributes.Attribute
	{
		#region All
		private bool _All;

		/// <summary>
		/// All properties are relevant. (use to override property names set by a base-class)
		/// </summary>
		public bool All
		{
			get { return _All; }
			set
			{
				if ( _All != value )
				{
					_All = value;
				}
			}
		}

		#endregion All

		#region PropertyNames
		private string[] _PropertyNames;

		/// <summary>
		/// Names of relevant properties.
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

		public SerializerRelevantPropertiesAttribute( bool all )
		{
			this.All = all;
			this.PropertyNames = null;
		}

		public SerializerRelevantPropertiesAttribute( params string[] properties )
		{
			this.PropertyNames = properties;
		}
	}
}
