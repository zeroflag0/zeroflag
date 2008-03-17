using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Converters = zeroflag.Serialization.Converters;

namespace zeroflag.Zml
{
	public class Manager
	{
		#region ApplyTo
		public virtual object ApplyTo(object value)
		{
			return this.ApplyTo<object>(value);
		}

		public virtual T ApplyTo<T>(T value)
		{
			value = (T)this.ApplyNode(value, null, zeroflag.Serialization.Descriptors.Descriptor.DoParse(value), value.GetType(), this.Document.DocumentElement);
			return value;
		}

		protected virtual object ApplyNode(object value, Action<object> setter, zeroflag.Serialization.Descriptors.Descriptor description, Type type, XmlNode node)
		{
			string text = node.Value;
			IFactory factory = null;
			if (Converters.String.Converter.CanConvert(type))
			{
				if (text != null && text.Trim() != "")
				{
					value = Converters.String.Converter.Parse(type, text);
				}
			}
			if (value == null && node.Attributes != null && node.Attributes["type"] != null && node.Attributes["type"].Value != null)
			{
				Type actual = TypeFinder.Instance[node.Attributes["type"].Value];
				value = TypeHelper.CreateInstance(actual);
			}
			if (value == null)
				try
				{
					value = TypeHelper.CreateInstance(type);
				}
				catch { }
			var descBack = description;
			if (value == null && (factory = this.FindFactory(type)) != null)
			{
				value = factory;
				type = value.GetType();
				description = zeroflag.Serialization.Descriptors.Descriptor.DoParse(value);
			}
			if (!Converters.String.Converter.CanConvert(type))
			{
				if (node.Attributes != null && !(typeof(System.Collections.IList).IsAssignableFrom(type) || typeof(IList<>).IsAssignableFrom(type)))
					foreach (XmlAttribute att in node.Attributes)
					{
						this.ApplyProperty(value, description, type, att, node);
					}

				foreach (XmlNode sub in node.ChildNodes)
				{
					if (typeof(System.Collections.IList).IsAssignableFrom(type) || typeof(IList<>).IsAssignableFrom(type))
					{
						// it's a collection, feed it...
						Type itemType = type.GetInterface("IList`1");
						object item = null;


						if (itemType == null)
						{
							List<System.Reflection.PropertyInfo> props = description.GetProperties(type);
							foreach (var p in props)
							{
								if (p.GetIndexParameters().Length > 0)
								{
									itemType = p.PropertyType;
									break;
								}
							}
							List<Type> types = TypeFinder.Instance.SearchAll(sub.Name, itemType);
							itemType = types[0];
							item = TypeHelper.CreateInstance(itemType);
						}
						else
						{
							itemType = type.GetInterface("IList`1").GetGenericArguments()[0];
							item = TypeHelper.CreateInstance(itemType);
						}

						//this.ApplyProperty(value, description, type, sub, node);
						this.ApplyNode(item, setter, zeroflag.Serialization.Descriptors.Descriptor.DoParse(item), itemType, sub);
					}
					else
						this.ApplyProperty(value, description, type, sub, node);
				}
			}

			if (value is IFactory)
			{
				value = ((IFactory)value).Create();
				type = value.GetType();
				description = descBack;
			}

			if (setter != null && value != null && !(typeof(System.Collections.IList).IsAssignableFrom(type) || typeof(IList<>).IsAssignableFrom(type)))
				try
				{
					setter(value);
				}
				catch (Exception exc)
				{
					try
					{
						setter(value);
					}
					catch
					{
					}
					Console.WriteLine(exc);
				}

			return value;
		}
		protected virtual void ApplyProperty(object value, zeroflag.Serialization.Descriptors.Descriptor description, Type type, XmlNode node, XmlNode parent)
		{
			bool direct = true;
			System.Reflection.PropertyInfo info = PropertyFinder.Instance[type, node.Name];
			if (info == null)
			{
				direct = false;
				info = PropertyFinder.Instance.SearchType(type, node.Name);
				if (info == null)
				{
					info = PropertyFinder.Instance[type, node.Name];
					return;
				}
			}

			zeroflag.Serialization.Descriptors.Descriptor inner = null;
			foreach (var desc in description.Inner)
			{
				if (desc != null && desc.Name == info.Name)
				{
					inner = desc;
					break;
				}
			}
			object inst = null;
			if (inner == null)
			{
				try
				{
					inner = zeroflag.Serialization.Descriptors.Descriptor.DoParse(inst = info.GetValue(value, null));
				}
				catch (Exception exc)
				{
					inst = null;
					Console.WriteLine(exc);
				}
				if (inner == null)
					inner = zeroflag.Serialization.Descriptors.Descriptor.DoParse(info.PropertyType);
			}

			if (info != null)
			{
				if (typeof(System.Collections.ICollection).IsAssignableFrom(info.PropertyType))// && TypeHelper.SpecializeType(typeof(ICollection<>), type).IsAssignableFrom(info.PropertyType))
				{
					if (inst == null)
						inst = info.GetValue(value, null);
					this.ApplyNode(inst, v =>
						{
							object instance = null;
							if (info.CanRead)
								instance = info.GetValue(value, null);
							if (instance == null && info.CanWrite)
							{
								instance = v;
								info.SetValue(value, instance, null);
							}
							else if (instance != null)
							{
								info.PropertyType.GetMethod("Add").Invoke(instance, new object[] { v });
							}
						}
						, inner, inner.Type, direct ? node : parent);
				}
				else
				{
					this.ApplyNode(inst, v => info.SetValue(value, v, null), inner, info.PropertyType, node);
				}
			}
		}
		#endregion ApplyTo

		#region GenerateFrom
		public virtual void GenerateFrom(object value)
		{
			this.GenerateNode(value, zeroflag.Serialization.Descriptors.Descriptor.DoParse(value), value.GetType(), this.Document.DocumentElement, null);
		}

		protected virtual XmlNode GenerateNode(object value, zeroflag.Serialization.Descriptors.Descriptor description, Type type, XmlNode node, XmlNode parent)
		{
			if (description == null || description.IsNull)
				return null;
			XmlDocument doc = this.Document;
			if (node == null)
				node = doc.CreateElement(description.Name ?? description.Type.Name.Split('`')[0]);

			bool passNodesToParent = false; ;
			//this.WriteAttribute(AttributeName, value.Name, node);
			//this.WriteAttribute(AttributeType, value.Type.FullName, node);
			//if (value.IsNull)
			//    this.WriteAttribute(AttributeNull, value.IsNull.ToString(), node);
			//if (value.Id > -1)
			//    this.WriteAttribute(AttributeId, value.Id.ToString(), node);
			//this.WriteAttribute("descriptor", value.ToString(), doc, node);

			if (!Converters.String.Converter.CanConvert(description.Value))
			{
				if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type.GetInterface("ICollection") != null)
				{
					// it's a collection...
					foreach (zeroflag.Serialization.Descriptors.Descriptor itemDesc in description.Inner)
					{
						itemDesc.Name = description.Name;
						this.GenerateNode(itemDesc.Value, itemDesc, itemDesc.Type, null, parent);
					}
					passNodesToParent = true;
				}
				else
				{
					int nonsense = 0;
					// generate attributes
					foreach (zeroflag.Serialization.Descriptors.Descriptor desc in description.Inner.FindAll(d => !d.IsNull && Converters.String.Converter.CanConvert(d.Value)))
					{
						this.WriteAttribute(desc.Name, Converters.String.Converter.Generate(desc.Type, desc.Value), node);
					}
					// generate elements...
					foreach (zeroflag.Serialization.Descriptors.Descriptor desc in description.Inner.FindAll(d => !d.IsNull && !Converters.String.Converter.CanConvert(d.Value)))
					{
						XmlNode inner = null;
						foreach (XmlNode i in node)
							if (i.Name.ToLower() == desc.Name.ToLower())
							{
								inner = i;
								break;
							}
						if (desc.Name != null && desc.Name != "")
						{
							if (typeof(System.Collections.IEnumerable).IsAssignableFrom(desc.Type) && desc.Type.GetInterface("ICollection") != null && desc.Inner.Count <= 0)
							{
							}
							else
								this.GenerateNode(desc.Value, desc, desc.Type, inner, node);
						}
						else
							nonsense++;
					}
					if (nonsense > 0)
					{
						//node.AppendChild(doc.CreateComment("found " + nonsense + " items without a proper name"));
					}
				}
			}
			else
			{
				//Console.WriteLine("Convert(" + value + ")");
				node.InnerText = Converters.String.Converter.Generate(description.Type, description.Value);

				//this.WriteAttribute("value", StringConverters.Base.Write(value.Value), doc, node);
			}

			if (!description.IsNull)
			{
				XmlNode target = parent ?? doc;
				if (!passNodesToParent)
				{
					target.AppendChild(node);
				}
				else
				{
					foreach (XmlNode child in node)
					{
						target.AppendChild(child);
					}
				}
			}

			return node;
		}

		protected XmlNode WriteAttribute(string name, string value, XmlNode parent)
		{
			XmlAttribute att = this.Document.CreateAttribute(name);
			att.Value = value ?? "";
			parent.Attributes.Append(att);
			return parent;
		}

		#endregion GenerateFrom

		#region GenerateXsd
#if TODO
		public virtual void GenerateXsd(Type type)
		{
			zeroflag.Serialization.Descriptors.Descriptor desc = zeroflag.Serialization.Descriptors.Descriptor.DoParse(type);
			this.GenerateXsd(desc, this.Document, null);
		}
		protected virtual void GenerateXsd(zeroflag.Serialization.Descriptors.Descriptor value, XmlDocument doc, XmlElement parent)
		{
			XmlElement node = doc.CreateElement(value.Name ?? value.Type.Name.Split('`')[0]);

				this.WriteAttribute("name", value.Name, doc, node);
				this.WriteAttribute("type", value.Type.FullName, doc, node);
			if (!Converters.String.Converter.CanConvert(value.Value))
			{
				//this.WriteAttribute("descriptor", value.ToString(), doc, node);

				int nonsense = 0;
				foreach (zeroflag.Serialization.Descriptors.Descriptor desc in value.Inner)
				{
					if (desc.Name != null && desc.Name != "")
						this.GenerateXsd(desc, doc, node);
					else
						nonsense++;
				}
				if (nonsense > 0)
				{
					node.AppendChild(doc.CreateComment("found " + nonsense + " items without a proper name"));
				}
			}
			else
			{
				//Console.WriteLine("Convert(" + value + ")");
				node.InnerText = Converters.String.Converter.Generate(value.Type, value.Value);

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
#endif
		#endregion GenerationXsd

		#region Factories

		private Dictionary<Type, IFactory> _Factories = default(Dictionary<Type, IFactory>);

		public Dictionary<Type, IFactory> Factories
		{
			get { return _Factories ?? (_Factories = this.FactoriesCreate); }
		}

		protected virtual Dictionary<Type, IFactory> FactoriesCreate
		{
			get
			{
				var facs = new Dictionary<Type, IFactory>();

				var types = TypeHelper.GetDerived(typeof(Factory<>));
				foreach (var type in types)
				{
					try
					{
						IFactory instance = TypeHelper.CreateInstance(type) as IFactory;
						if (instance != null)
							facs.Add(instance.ProductType, instance);
					}
					catch (Exception exc)
					{
						Console.WriteLine(exc);
					}
				}

				return facs;
			}
		}

		public IFactory FindFactory(Type type)
		{
			if (type == null)
				return null;
			if (this.Factories.ContainsKey(type) && this.Factories[type] != null)
				return this.Factories[type];
			else
				return this.Factories[type] = this.FindFactory(type.BaseType);
		}
		#endregion Factories

		#region XmlDocument
		XmlDocument _Document = new XmlDocument();

		public XmlDocument Document
		{
			get { return _Document; }
		}

		#region System.Xml.XPath.IXPathNavigable

		public virtual System.Xml.XPath.XPathNavigator CreateNavigator()
		{
			return this.Document.CreateNavigator();
		}

		#endregion System.Xml.XPath.IXPathNavigable


		#region System.Xml.XmlDocument

		//public virtual System.Xml.XmlNodeType NodeType
		//{
		//    get { return this.Document.NodeType; }
		//}

		//public virtual System.Xml.XmlNode ParentNode
		//{
		//    get { return this.Document.ParentNode; }
		//}

		//public virtual System.Xml.XmlDocumentType DocumentType
		//{
		//    get { return this.Document.DocumentType; }
		//}

		//public System.Xml.XmlImplementation Implementation
		//{
		//    get { return this.Document.Implementation; }
		//}

		//public virtual System.String Name
		//{
		//    get { return this.Document.Name; }
		//}

		//public virtual System.String LocalName
		//{
		//    get { return this.Document.LocalName; }
		//}

		//public System.Xml.XmlElement DocumentElement
		//{
		//    get { return this.Document.DocumentElement; }
		//}

		//public virtual System.Xml.XmlDocument OwnerDocument
		//{
		//    get { return this.Document.OwnerDocument; }
		//}

		//public System.Xml.Schema.XmlSchemaSet Schemas
		//{
		//    get { return this.Document.Schemas; }
		//    set { this.Document.Schemas = value; }
		//}

		//public virtual System.Xml.XmlResolver XmlResolver
		//{
		//    set { this.Document.XmlResolver = value; }
		//}

		//public System.Xml.XmlNameTable NameTable
		//{
		//    get { return this.Document.NameTable; }
		//}

		//public bool PreserveWhitespace
		//{
		//    get { return this.Document.PreserveWhitespace; }
		//    set { this.Document.PreserveWhitespace = value; }
		//}

		//public virtual bool IsReadOnly
		//{
		//    get { return this.Document.IsReadOnly; }
		//}

		//public virtual System.String InnerXml
		//{
		//    get { return this.Document.InnerXml; }
		//    set { this.Document.InnerXml = value; }
		//}

		//public virtual System.Xml.Schema.IXmlSchemaInfo SchemaInfo
		//{
		//    get { return this.Document.SchemaInfo; }
		//}

		//public virtual System.String BaseURI
		//{
		//    get { return this.Document.BaseURI; }
		//}

		//public virtual System.String Value
		//{
		//    get { return this.Document.Value; }
		//    set { this.Document.Value = value; }
		//}

		//public virtual System.Xml.XmlNodeList ChildNodes
		//{
		//    get { return this.Document.ChildNodes; }
		//}

		//public virtual System.Xml.XmlNode PreviousSibling
		//{
		//    get { return this.Document.PreviousSibling; }
		//}

		//public virtual System.Xml.XmlNode NextSibling
		//{
		//    get { return this.Document.NextSibling; }
		//}

		//public virtual System.Xml.XmlAttributeCollection Attributes
		//{
		//    get { return this.Document.Attributes; }
		//}

		//public virtual System.Xml.XmlNode FirstChild
		//{
		//    get { return this.Document.FirstChild; }
		//}

		//public virtual System.Xml.XmlNode LastChild
		//{
		//    get { return this.Document.LastChild; }
		//}

		//public virtual bool HasChildNodes
		//{
		//    get { return this.Document.HasChildNodes; }
		//}

		//public virtual System.String NamespaceURI
		//{
		//    get { return this.Document.NamespaceURI; }
		//}

		//public virtual System.String Prefix
		//{
		//    get { return this.Document.Prefix; }
		//    set { this.Document.Prefix = value; }
		//}

		//public virtual System.String InnerText
		//{
		//    get { return this.Document.InnerText; }
		//    set { this.Document.InnerText = value; }
		//}

		//public virtual System.String OuterXml
		//{
		//    get { return this.Document.OuterXml; }
		//}

		//public virtual System.Xml.XmlElement this[System.String name]
		//{
		//    get { return this.Document[name]; }
		//}

		//public event System.Xml.XmlNodeChangedEventHandler NodeInserting
		//{
		//    add { this.Document.NodeInserting += value; }
		//    remove { this.Document.NodeInserting -= value; }
		//}

		//public event System.Xml.XmlNodeChangedEventHandler NodeInserted
		//{
		//    add { this.Document.NodeInserted += value; }
		//    remove { this.Document.NodeInserted -= value; }
		//}

		//public event System.Xml.XmlNodeChangedEventHandler NodeRemoving
		//{
		//    add { this.Document.NodeRemoving += value; }
		//    remove { this.Document.NodeRemoving -= value; }
		//}

		//public event System.Xml.XmlNodeChangedEventHandler NodeRemoved
		//{
		//    add { this.Document.NodeRemoved += value; }
		//    remove { this.Document.NodeRemoved -= value; }
		//}

		//public event System.Xml.XmlNodeChangedEventHandler NodeChanging
		//{
		//    add { this.Document.NodeChanging += value; }
		//    remove { this.Document.NodeChanging -= value; }
		//}

		//public event System.Xml.XmlNodeChangedEventHandler NodeChanged
		//{
		//    add { this.Document.NodeChanged += value; }
		//    remove { this.Document.NodeChanged -= value; }
		//}

		//public virtual System.Xml.XmlNode CloneNode(bool deep)
		//{
		//    return this.Document.CloneNode(deep);
		//}

		//public virtual System.Xml.XmlDocumentType CreateDocumentType(System.String name, System.String publicId, System.String systemId, System.String internalSubset)
		//{
		//    return this.Document.CreateDocumentType(name, publicId, systemId, internalSubset);
		//}

		//public virtual System.Xml.XmlDocumentFragment CreateDocumentFragment()
		//{
		//    return this.Document.CreateDocumentFragment();
		//}

		//public virtual System.Xml.XmlEntityReference CreateEntityReference(System.String name)
		//{
		//    return this.Document.CreateEntityReference(name);
		//}

		//public virtual System.Xml.XmlProcessingInstruction CreateProcessingInstruction(System.String target, System.String data)
		//{
		//    return this.Document.CreateProcessingInstruction(target, data);
		//}

		//public virtual System.Xml.XmlDeclaration CreateXmlDeclaration(System.String version, System.String encoding, System.String standalone)
		//{
		//    return this.Document.CreateXmlDeclaration(version, encoding, standalone);
		//}

		//public virtual System.Xml.XmlText CreateTextNode(System.String text)
		//{
		//    return this.Document.CreateTextNode(text);
		//}

		//public virtual System.Xml.XmlNodeList GetElementsByTagName(System.String name)
		//{
		//    return this.Document.GetElementsByTagName(name);
		//}

		//public virtual System.Xml.XmlElement GetElementById(System.String elementId)
		//{
		//    return this.Document.GetElementById(elementId);
		//}

		//public virtual System.Xml.XmlNode ImportNode(System.Xml.XmlNode node, bool deep)
		//{
		//    return this.Document.ImportNode(node, deep);
		//}

		//public virtual System.Xml.XmlNode CreateNode(System.Xml.XmlNodeType type, System.String prefix, System.String name, System.String namespaceURI)
		//{
		//    return this.Document.CreateNode(type, prefix, name, namespaceURI);
		//}

		//public virtual System.Xml.XmlNode ReadNode(System.Xml.XmlReader reader)
		//{
		//    return this.Document.ReadNode(reader);
		//}

		public virtual void Load(System.IO.Stream inStream)
		{
			this.Document.Load(inStream);
		}

		public virtual void Load(string file)
		{
			this.Document.Load(file);
		}

		public virtual void LoadXml(System.String xml)
		{
			this.Document.LoadXml(xml);
		}

		public virtual void Save(System.String filename)
		{
			this.Document.Save(filename);
		}

		public virtual void WriteTo(System.Xml.XmlWriter w)
		{
			this.Document.WriteTo(w);
		}

		public virtual void WriteContentTo(System.Xml.XmlWriter xw)
		{
			this.Document.WriteContentTo(xw);
		}

		//public System.Xml.XmlAttribute CreateAttribute(System.String name)
		//{
		//    return this.Document.CreateAttribute(name);
		//}

		//public virtual System.Xml.XmlCDataSection CreateCDataSection(System.String data)
		//{
		//    return this.Document.CreateCDataSection(data);
		//}

		//public virtual System.Xml.XmlComment CreateComment(System.String data)
		//{
		//    return this.Document.CreateComment(data);
		//}

		//public System.Xml.XmlElement CreateElement(System.String name)
		//{
		//    return this.Document.CreateElement(name);
		//}

		//public virtual System.Xml.XmlSignificantWhitespace CreateSignificantWhitespace(System.String text)
		//{
		//    return this.Document.CreateSignificantWhitespace(text);
		//}

		//public virtual System.Xml.XmlWhitespace CreateWhitespace(System.String text)
		//{
		//    return this.Document.CreateWhitespace(text);
		//}

		//public void Validate(System.Xml.Schema.ValidationEventHandler validationEventHandler)
		//{
		//    this.Document.Validate(validationEventHandler);
		//}

		//public virtual System.Xml.XmlNode InsertBefore(System.Xml.XmlNode newChild, System.Xml.XmlNode refChild)
		//{
		//    return this.Document.InsertBefore(newChild, refChild);
		//}

		//public virtual System.Xml.XmlNode InsertAfter(System.Xml.XmlNode newChild, System.Xml.XmlNode refChild)
		//{
		//    return this.Document.InsertAfter(newChild, refChild);
		//}

		//public virtual System.Xml.XmlNode ReplaceChild(System.Xml.XmlNode newChild, System.Xml.XmlNode oldChild)
		//{
		//    return this.Document.ReplaceChild(newChild, oldChild);
		//}

		//public virtual System.Xml.XmlNode RemoveChild(System.Xml.XmlNode oldChild)
		//{
		//    return this.Document.RemoveChild(oldChild);
		//}

		//public virtual System.Xml.XmlNode PrependChild(System.Xml.XmlNode newChild)
		//{
		//    return this.Document.PrependChild(newChild);
		//}

		//public virtual System.Xml.XmlNode AppendChild(System.Xml.XmlNode newChild)
		//{
		//    return this.Document.AppendChild(newChild);
		//}

		//public virtual void Normalize()
		//{
		//    this.Document.Normalize();
		//}

		//public virtual bool Supports(System.String feature, System.String version)
		//{
		//    return this.Document.Supports(feature, version);
		//}

		//public virtual void RemoveAll()
		//{
		//    this.Document.RemoveAll();
		//}

		//public virtual System.String GetNamespaceOfPrefix(System.String prefix)
		//{
		//    return this.Document.GetNamespaceOfPrefix(prefix);
		//}

		//public virtual System.String GetPrefixOfNamespace(System.String namespaceURI)
		//{
		//    return this.Document.GetPrefixOfNamespace(namespaceURI);
		//}

		//public System.Xml.XmlNode SelectSingleNode(System.String xpath)
		//{
		//    return this.Document.SelectSingleNode(xpath);
		//}

		//public System.Xml.XmlNodeList SelectNodes(System.String xpath)
		//{
		//    return this.Document.SelectNodes(xpath);
		//}

		//public System.Type GetType()
		//{
		//    return this.Document.GetType();
		//}

		//public virtual System.String ToString()
		//{
		//    return this.Document.ToString();
		//}

		//public virtual bool Equals(System.Object obj)
		//{
		//    return this.Document.Equals(obj);
		//}

		//public virtual int GetHashCode()
		//{
		//    return this.Document.GetHashCode();
		//}

		#endregion System.Xml.XmlDocument
		#endregion XmlDocument
	}
}
