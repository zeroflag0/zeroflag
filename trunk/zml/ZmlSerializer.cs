using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using zeroflag.Serialization;
using zeroflag.Serialization.Descriptors;
using Converters = zeroflag.Serialization.Converters.String;

namespace zeroflag.Zml
{
	public class ZmlSerializer : zeroflag.Serialization.Serializer
	{
		const string AttributeName = "_name";
		const string AttributeType = "_type";
		const string AttributeNull = "_null";
		const string AttributeId = "_id";

		#region Serialize
		public override void Serialize(zeroflag.Serialization.Descriptors.Descriptor value)
		{
			XmlDocument doc = new XmlDocument();

			doc.AppendChild(doc.CreateXmlDeclaration("1.0", null, null));
			doc.AppendChild(doc.CreateComment(value.ToStringTree().ToString()));
			//doc.AppendChild(doc.CreateElement("root"));

			this.Serialize(value, doc, null, doc.DocumentElement);
			doc.Save(this.FileName);
		}

		protected virtual void Serialize(zeroflag.Serialization.Descriptors.Descriptor value, XmlDocument doc, zeroflag.Serialization.Descriptors.Descriptor valueParent, XmlElement xmlParent)
		{
			if (value == null || value.IsNull ||
				(typeof(System.Collections.ICollection).IsAssignableFrom(value.Type) && value.Inner.Count <= 0 && this.HideUnused)
				)
				return;


			string name = value.Name ?? value.Type.Name.Split('`')[0];
			CWL("Serialize(" + value + ")");
			if (!this.Converters.CanConvert<string>(value.Value))// || value.Name == null)
			{
				// complex type...
				XmlElement node = doc.CreateElement(name);

				bool explicitType = false;

				if (valueParent != null && valueParent.Value != null && value.Name != null)
				{
					var info = valueParent.Type.GetProperty(value.Name);
					if (info != null)
					{
						explicitType = info.PropertyType != value.Type;
					}
				}

				if (explicitType)
					this.WriteAttribute(AttributeType, value.Type.FullName, doc, node);
				//if (value.IsNull)
				//    this.WriteAttribute(AttributeNull, value.IsNull.ToString(), doc, node);
				if (value.Id > -1 && value.IsReferenced)
					this.WriteAttribute(AttributeId, value.Id.ToString(), doc, node);
				//#if DEBUG
				//            this.WriteAttribute("descriptor", value.ToString(), doc, node);
				//#endif
				if (!this.Converters.CanConvert<string>(value.Value))
				{

					foreach (zeroflag.Serialization.Descriptors.Descriptor desc in value.Inner)
					{
						this.Serialize(desc, doc, value, node);
					}
				}

				if (!value.IsNull && this.HideUnused)
				{
					if (xmlParent != null)
						xmlParent.AppendChild(node);
					else
						doc.AppendChild(node);
				}
			}
			else if (value.Name == null)
			{
				XmlElement node = doc.CreateElement(name);
				bool explicitType = false;

				if (valueParent != null && valueParent.Value != null && value.Name != null)
				{
					var info = valueParent.Type.GetProperty(value.Name);
					if (info != null)
					{
						explicitType = info.PropertyType != value.Type;
					}
				}

				if (explicitType)
					this.WriteAttribute(AttributeType, value.Type.FullName, doc, node);

				node.InnerText = this.Converters.Generate<string>(value.Type, value.Value);

				if (!value.IsNull && this.HideUnused)
				{
					if (xmlParent != null)
						xmlParent.AppendChild(node);
					else
						doc.AppendChild(node);
				}
			}
			else
			{
				XmlAttribute node = doc.CreateAttribute(value.Name);
				//Console.WriteLine("Convert(" + value + ")");
				node.Value = this.Converters.Generate<string>(value.Type, value.Value);

				//this.WriteAttribute("value", StringConverters.Base.Write(value.Value), doc, node);

				if (!value.IsNull && this.HideUnused)
				{
					if (xmlParent != null)
						xmlParent.Attributes.Append(node);
					else
						doc.Attributes.Append(node);
				}
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
		public override object Deserialize(object value, zeroflag.Serialization.Descriptors.Descriptor desc)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(this.FileName);

			value = this.Deserialize(value, desc, doc.DocumentElement);

			Console.WriteLine("<Deserialized>");
			Console.WriteLine(desc.ToStringTree().ToString());
			Console.WriteLine("</Deserialized>");
			Console.WriteLine("<Created>");
			Console.WriteLine(zeroflag.Serialization.Descriptors.Descriptor.DoParse(value).ToStringTree().ToString());
			Console.WriteLine("</Created>");

			return value;
		}
		int depth = 0;
		protected virtual object Deserialize(object value, Descriptor desc, XmlNode node)
		{
			depth++;

			string explicitType = this.GetAttribute(AttributeType, node);
			if (desc.Name == null)
			{
				//desc.Name = this.GetAttribute(AttributeName, node) ?? node.Name;


				explicitType = explicitType ?? node.Name;
			}

			if (explicitType != null)
			{
				var types = TypeFinder.Instance.SearchAll(explicitType, desc.Type);
				if (types.Count > 0)
				{
					Type type = types.Find(t => t.Name != null && t.Name.ToLower() == explicitType.ToLower()) ?? desc.Type;
					if (type != desc.Type)
					{
						desc = zeroflag.Serialization.Descriptors.Descriptor.GetDescriptor(type).Parse(desc.Name, type, desc.Owner);
					}
				}
			}

			desc.Value = value;
			if (desc.Value == null && desc.Name != null && desc.Owner != null && desc.Owner.Value != null)
			{
				var info = desc.Owner.Property(desc.Name);
				if (info != null && info.GetIndexParameters().Length == 0)
					value = desc.Value = info.GetValue(desc.Owner.Value, null);
			}
			if (value != null && value.GetType() != desc.Type && !desc.Type.IsAssignableFrom(value.GetType()))
				value = null;

			if (this.GetAttribute(AttributeNull, node) != null)
				desc.IsNull = false;
			else
			{
				bool isnull = false;
				bool.TryParse(this.GetAttribute(AttributeNull, node), out isnull);
				desc.IsNull = isnull;
			}

			if (this.GetAttribute(AttributeId, node) != null)
			{
				int id;
				int.TryParse(this.GetAttribute(AttributeId, node), out id);
				desc.Id = id;
			}


			desc.Value = value;

			desc.GenerateParse();
			desc.GenerateCreate();


			CWL(new StringBuilder().Append(' ', depth).Append("Deserialize(name='" + desc.Name + "', type='" + desc.Type + "', isnull='" + desc.IsNull + "', id='" + desc.Id + "', value='" + desc.Value + "', children='" + node.ChildNodes.Count + "')").ToString());
			//foreach (Descriptor cr in desc.Generated.Values) Console.WriteLine("\tid=" + cr.Id + ", name=" + cr.Name + ", type=" + cr.Type + ", value=" + cr.Value);


			List<XmlNode> nodes = new List<XmlNode>();
			if (node.Attributes != null)
				foreach (XmlNode n in node.Attributes)
					if (n.Name != AttributeType)
						nodes.Add(n);
			foreach (XmlNode n in node.ChildNodes)
				nodes.Add(n);

			foreach (XmlNode sub in nodes)
			{
				if (sub is XmlText)
				{
					string text = ((XmlText)sub).Value;
					if (this.Converters.CanConvert<string>(desc.Type))
					{
						desc.Value = this.Converters.Parse<string>(desc.Type, text);
					}
					else
					{
						try
						{
							Descriptor inner = desc.Inner.Find(i => i.Name != null && i.Name.ToLower() == "value");
							if (inner != null)
								inner.Value = this.Converters.Parse<string>(typeof(string), text);
							//desc.Property("Value").SetValue(desc.Value, this.Converters.Parse<string>(typeof(string), text), null);
						}
						catch
						{
						}
					}
				}
				else //if (sub is XmlElement)
				{
					string subTypeName = this.GetAttribute(AttributeType, sub) ?? sub.Name;
					Type subType = null;
					Descriptor inner = null;
					inner = desc.Inner.Find(i => i.Name != null && i.Name.ToLower() == sub.Name.ToLower());
					// try to find the property in the parent descriptor...
					if (inner != null)
						subType = inner.Type;
					else
					{
						// try to find the property on the type...
						//var info = new List<System.Reflection.PropertyInfo>(desc.Type.GetProperties()).Find(i => i.Name.ToLower() == sub.Name.ToLower());
						var info = desc.Property(subTypeName);
						if (info != null)
						{
							subType = info.PropertyType;
							//subTypeName = info.Name;
						}
						else
						{
							// try to find the type...
							subType = TypeFinder.Instance[subTypeName];
						}
					}

					if (subType == null)
						continue;

					if (inner == null)
						inner = Descriptor.DoParse(subType, desc);
					if (inner != null && !desc.Inner.Contains(inner))
						desc.Inner.Add(inner);
					//if (subTypeName != null)
					//    inner.Name = subTypeName;
					this.Deserialize(inner.Value, inner, sub);
				}
			}
			depth--;

			return desc.GenerateLink();
			//return desc.Generate();
		}

		string GetAttribute(string name, XmlNode node)
		{
			try
			{
				return node.Attributes[name].Value;
			}
			catch (Exception exc1)
			{
				try
				{
					foreach (XmlAttribute att in node.Attributes)
					{
						if (att.Name != null && att.Name.ToLower() == name.ToLower())
							return att.Value;
					}
					//Console.WriteLine(exc1);
					return null;
				}
				catch (Exception exc)
				{
					//Console.WriteLine(exc);
					return null;
				}
			}
		}
		#endregion Deserialize


		#region HideUnused

		private bool _HideUnused = true;

		public bool HideUnused
		{
			get { return _HideUnused; }
			set
			{
				if (_HideUnused != value)
				{
					_HideUnused = value;
				}
			}
		}
		#endregion HideUnused

		public ZmlSerializer()
		{
		}

		public ZmlSerializer(string fileName)
			: base(fileName)
		{
		}

		public ZmlSerializer(ZmlSerializer parent)
			: base(parent)
		{
		}

		[System.Diagnostics.Conditional("VERBOSE")]
		static internal void CWL(object value)
		{
			Console.WriteLine(value);
		}

	}
}
