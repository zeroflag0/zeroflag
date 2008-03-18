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
using zeroflag.Serialization.Descriptors;

namespace zeroflag.Serialization
{
	public class XmlSerializer : Serializer
	{
		//public override object Deserialize(ObjectDescription type)
		//{
		//    throw new NotImplementedException();
		//}
		const string AttributeName = "name";
		const string AttributeType = "type";
		const string AttributeNull = "null";
		const string AttributeId = "id";

		#region Serialize
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
			Descriptor.CWL("Serialize(" + value + ")");
			XmlElement node = doc.CreateElement(value.Name ?? value.Type.Name.Split('`')[0]);

			this.WriteAttribute(AttributeName, value.Name, doc, node);
			this.WriteAttribute(AttributeType, value.Type.FullName, doc, node);
			if (value.IsNull)
				this.WriteAttribute(AttributeNull, value.IsNull.ToString(), doc, node);
			if (value.Id > -1)
				this.WriteAttribute(AttributeId, value.Id.ToString(), doc, node);
#if DEBUG
			this.WriteAttribute("descriptor", value.ToString(), doc, node);
#endif

			if (!this.Converters.CanConvert<string>(value.Value))// Converters.String.Converter.CanConvert(value.Value))
			{
				int nonsense = 0;
				foreach (zeroflag.Serialization.Descriptors.Descriptor desc in value.Inner)
				{
					//if (desc.Name != null && desc.Name != "")
					this.Serialize(desc, doc, node);
					//else
					//    nonsense++;
				}
				if (nonsense > 0)
				{
					Descriptor.CWL("found " + nonsense + " items without a proper name");
					node.AppendChild(doc.CreateComment("found " + nonsense + " items without a proper name"));
				}
			}
			else
			{
				//Console.WriteLine("Convert(" + value + ")");
				//node.InnerText = Converters.String.Converter.Generate(value.Type, value.Value);
				node.InnerText = this.Converters.Generate<string>(value.Type, value.Value);

				//this.WriteAttribute("value", StringConverters.Base.Write(value.Value), doc, node);
			}

			if (!value.IsNull)
			{
				if (parent != null)
					parent.AppendChild(node);
				else
					doc.AppendChild(node);
			}
		}

		protected XmlElement WriteAttribute(string name, string value, XmlDocument doc, XmlElement parent)
		{
			XmlAttribute att = doc.CreateAttribute(name);
			att.Value = value ?? "";
			parent.Attributes.Append(att);
			return parent;
		}

		#endregion Serialize

		#region Deserialize
		public override object Deserialize(zeroflag.Serialization.Descriptors.Descriptor desc)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(this.FileName);

			return this.Deserialize(null, desc, doc.DocumentElement);
		}
		int depth = 0;
		protected virtual object Deserialize(object value, Descriptor desc, XmlElement node)
		{
			depth++;
			if (desc.Name == null)
				desc.Name = this.GetAttribute(AttributeName, node) ?? node.Name;

			if (node.Attributes[AttributeNull] != null)
			{
				bool isnull = false;
				bool.TryParse(this.GetAttribute(AttributeNull, node), out isnull);
				desc.IsNull = isnull;
			}
			else
				desc.IsNull = false;

			if (node.Attributes[AttributeId] != null)
			{
				int id;
				int.TryParse(this.GetAttribute(AttributeId, node), out id);
				desc.Id = id;
			}


			desc.Value = value;
			//desc.PreGenerate();
			//desc.DoCreateInstance();

			Descriptor.CWL(new StringBuilder().Append(' ', depth).Append("Deserialize(name='" + desc.Name + "', type='" + desc.Type + "', isnull='" + desc.IsNull + "', id='" + desc.Id + "', value='" + desc.Value + "', children='" + node.ChildNodes.Count + "')").ToString());
			//foreach (Descriptor cr in desc.Generated.Values) Console.WriteLine("\tid=" + cr.Id + ", name=" + cr.Name + ", type=" + cr.Type + ", value=" + cr.Value);

			foreach (XmlNode sub in node.ChildNodes)
			{
				if (sub is XmlElement)
				{
					Type subType = TypeHelper.GetType(this.GetAttribute(AttributeType, sub as XmlElement));
					Descriptor inner = Descriptor.DoParse(subType, desc);
					this.Deserialize(null, inner, sub as XmlElement);
				}
				else if (sub is XmlText)
				{
					string text = ((XmlText)sub).Value;
					desc.Value = this.Converters.Parse(desc.Type, text);
				}
			}
			depth--;
			desc.GenerateParse();
			desc.GenerateCreate();

			return desc.GenerateLink();
			//return desc.Generate();
		}

		string GetAttribute(string name, XmlElement node)
		{
			try
			{
				return node.Attributes[name].Value;
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
				return null;
			}
		}
		#endregion Deserialize


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
