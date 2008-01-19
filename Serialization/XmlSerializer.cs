using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace zeroflag.Serialization
{
	public class XmlSerializer : Serializer
	{
		public override object Deserialize(ObjectDescription type)
		{
			throw new NotImplementedException();
		}

		protected override void Serialize(ObjectDescription value)
		{
			XmlDocument doc = new XmlDocument();

			doc.CreateXmlDeclaration("1.0", null, null);
			doc.AppendChild(doc.CreateElement("root"));

			this.Serialize(value, doc, doc.DocumentElement);

			doc.Save(this.FileName);
		}

		protected virtual void Serialize(ObjectDescription value, XmlDocument doc, XmlElement parent)
		{
			XmlElement node = doc.CreateElement(value.Name ?? value.Type.Name.Split('`')[0]);

			this.WriteAttribute("name", value.Name, doc, node);
			this.WriteAttribute("type", value.Type.FullName, doc, node);

			if (value.Properties.Count > 0)
			{
				foreach (ObjectDescription desc in value.Properties)
				{
					this.Serialize(desc, doc, node);
				}
			}
			else
			{
				node.InnerText = StringConverters.Base.Write(value.Value);
				//this.WriteAttribute("value", StringConverters.Base.Write(value.Value), doc, node);
			}

			parent.AppendChild(node);
		}

		protected XmlElement WriteAttribute(string name, string value, XmlDocument doc, XmlElement parent)
		{
			XmlAttribute att = doc.CreateAttribute(name);
			att.Value = value ?? "";
			parent.Attributes.Append(att);
			return parent;
		}

		protected override Serializer CreateChild()
		{
			return new XmlSerializer(this);
		}

		public XmlSerializer()
		{
		}

		public XmlSerializer(string fileName)
			: base(fileName)
		{
		}

		public XmlSerializer(XmlSerializer parent)
			: base(parent)
		{
		}
	}
}
