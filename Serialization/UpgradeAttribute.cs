using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization
{
	public class UpgradeAttribute : SerializationAttribute
	{
		string[] _Previous = new string[0];

		public string[] Previous
		{
			get { return _Previous; }
			set { _Previous = value; }
		}

		public UpgradeAttribute(params string[] from)
		{
			this.Previous = from;
		}

		public override void ApplyTo(zeroflag.Serialization.Descriptors.Descriptor desc)
		{
			Type type = desc.Type;
			List<string> previous = new List<string>(this.Previous);
			foreach (System.Reflection.PropertyInfo prop in type.GetProperties())
			{
				if (previous.Contains(prop.Name))
				{
					desc.Name = prop.Name;
					break;
				}
			}
		}
	}
}
