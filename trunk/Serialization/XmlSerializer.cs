#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2008  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************
#endregion LGPL License

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace zeroflag.Serialization
{
	public class XmlSerializer : Serializer
	{
		//public override object Deserialize(ObjectDescription type)
		//{
		//    throw new NotImplementedException();
		//}

		public override void Serialize(zeroflag.Serialization.Descriptors.Descriptor value)
		{
			XmlDocument doc = new XmlDocument();

			doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));
			//doc.AppendChild(doc.CreateElement("root"));

			this.Serialize(value, doc, doc.DocumentElement);

			doc.Save(this.FileName);
		}

		protected virtual void Serialize(zeroflag.Serialization.Descriptors.Descriptor value, XmlDocument doc, XmlElement parent)
		{
			Console.WriteLine("Serialize(" + value + ")");
			XmlElement node = doc.CreateElement(value.Name ?? value.Type.Name.Split('`')[0]);

			this.WriteAttribute("name", value.Name, doc, node);
			this.WriteAttribute("type", value.Type.FullName, doc, node);
			if (value.Id > -1)
				this.WriteAttribute("id", value.Id.ToString(), doc, node);
			//this.WriteAttribute("descriptor", value.ToString(), doc, node);

			if (!Converters.String.Converter.CanConvert(value.Value))
			{
				foreach (zeroflag.Serialization.Descriptors.Descriptor desc in value.Inner)
				{
					this.Serialize(desc, doc, node);
				}
			}
			else
			{
				Console.WriteLine("Convert(" + value + ")");
					node.InnerText = Converters.String.Converter.Generate(value.Value);

				//this.WriteAttribute("value", StringConverters.Base.Write(value.Value), doc, node);
			}

			if (parent != null)
				parent.AppendChild(node);
			else
				doc.AppendChild(node);
		}

		protected XmlElement WriteAttribute(string name, string value, XmlDocument doc, XmlElement parent)
		{
			XmlAttribute att = doc.CreateAttribute(name);
			att.Value = value ?? "";
			parent.Attributes.Append(att);
			return parent;
		}

		//protected override Serializer CreateChild()
		//{
		//    return new XmlSerializer(this);
		//}

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
