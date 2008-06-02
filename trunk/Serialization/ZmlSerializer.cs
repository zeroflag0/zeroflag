﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using zeroflag.Serialization;
using zeroflag.Serialization.Descriptors;
using Converters = zeroflag.Serialization.Converters.String;

namespace zeroflag.Serialization
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
			//doc.AppendChild(doc.CreateComment(value.ToStringTree().ToString()));
			//doc.AppendChild(doc.CreateElement("root"));

			this.Serialize(value, doc, null, doc.DocumentElement, new List<int>());
			doc.Save(this.FileName);
		}

		protected virtual void Serialize(zeroflag.Serialization.Descriptors.Descriptor desc, XmlDocument doc, zeroflag.Serialization.Descriptors.Descriptor valueParent, XmlElement xmlParent, List<int> ids)
		{
			if (desc == null || desc.IsNull)
				return;

			//desc.Inner.Clear();
			//desc.Parse(desc.Name, desc.Type, desc.Value);
			bool explicitType = false;
			if (desc.Value != null && desc.Value.GetType() != desc.Type)
			{
				desc.Type = desc.Value.GetType();
				//explicitType = true;
			}

			string name = desc.Name ?? desc.Type.Name.Split('`')[0];
			CWL("Serialize(" + desc + ")");
			if (!this.Converters.CanConvert<string>(desc.Value))// || value.Name == null)
			{
				// complex type...
				XmlElement node = doc.CreateElement(name);

				if (valueParent != null && valueParent.Value != null && desc.Name != null)
				{
					var info = valueParent.Type.GetProperty(desc.Name);
					if (info != null)
					{
						explicitType = info.PropertyType != desc.Type;
					}
				}

				//if (!this.Converters.CanConvert<string>(desc.Value))
				//{

				//    foreach (zeroflag.Serialization.Descriptors.Descriptor inner in desc.Inner)
				//    {
				//        this.Serialize(inner, doc, desc, node);
				//    }
				//}

				if (!(desc.IsNull && this.HideUnused))
				{
					if (xmlParent != null)
						xmlParent.AppendChild(node);
					else
						doc.AppendChild(node);
				}

				bool hasId = desc.Id > -1 && desc.IsReferenced;
				bool isReference = false;
				if (desc.Id > -1 && desc.IsReferenced)
				{
					this.WriteAttribute(AttributeId, desc.Id.ToString(), doc, node);
					if (ids.Contains(desc.Id ?? -1))
						isReference = true;
					else
						ids.Add(desc.Id ?? -1);
				}

				if (!isReference)
				{
					// serialize inner...
					List<Descriptor> complex = new List<Descriptor>();
					foreach (Descriptor inner in desc.Inner)
					{
						if (inner.Name == null || !this.Converters.CanConvert<string>(inner.Value))
						{
							// complex type...
							complex.Add(inner);
						}
						else
						{
							// simple type (attribute)...
							this.Serialize(inner, doc, desc, node, ids);
						}
					}
					if (explicitType)
						this.WriteAttribute(AttributeType, desc.Type.FullName, doc, node);
					//if (value.IsNull)
					//    this.WriteAttribute(AttributeNull, value.IsNull.ToString(), doc, node);

					//#if DEBUG
					//            this.WriteAttribute("descriptor", value.ToString(), doc, node);
					//#endif

					foreach (Descriptor inner in complex)
						this.Serialize(inner, doc, desc, node, ids);
				}
			}
			else if (desc.Name == null)
			{
				XmlElement node = doc.CreateElement(name);

				if (valueParent != null && valueParent.Value != null && desc.Name != null)
				{
					var info = valueParent.Type.GetProperty(desc.Name);
					if (info != null)
					{
						explicitType = info.PropertyType != desc.Type;
					}
				}

				if (explicitType)
					this.WriteAttribute(AttributeType, desc.Type.FullName, doc, node);

				node.InnerText = this.Converters.Generate<string>(desc.Type, desc.Value);

				if (!(desc.IsNull && this.HideUnused))
				{
					if (xmlParent != null)
						xmlParent.AppendChild(node);
					else
						doc.AppendChild(node);
				}
			}
			else
			{
				XmlAttribute node = doc.CreateAttribute(desc.Name);
				//Console.WriteLine("Convert(" + value + ")");
				node.Value = this.Converters.Generate<string>(desc.Type, desc.Value);

				//this.WriteAttribute("value", StringConverters.Base.Write(value.Value), doc, node);

				if (!(desc.IsNull && this.HideUnused))
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

			value = this.Deserialize(value, desc, null, doc.DocumentElement);

			//Console.WriteLine("<Deserialized>");
			//Console.WriteLine(desc.ToStringTree().ToString());
			//Console.WriteLine("</Deserialized>");
			//Console.WriteLine("<Created>");
			//Console.WriteLine(this.Context.Parse(value).ToStringTree().ToString());
			//Console.WriteLine("</Created>");

			return value;
		}
		int depth = 0;
		protected virtual object Deserialize(object value, Descriptor desc, Descriptor outer, XmlNode node)
		{
			depth++;
			//Benchmark.Instance.Trace("Deserialize", desc, node);
			string explicitType = this.GetAttribute(AttributeType, node);
			if (desc.Name == null)
			{
				//desc.Name = this.GetAttribute(AttributeName, node) ?? node.Name;


				explicitType = explicitType ?? node.Name;
			}

			if (explicitType != null)
			{
				//Benchmark.Instance.Trace("TypeFinder.Instance"); 
				Type type = TypeFinder.Instance[explicitType, desc.Type];
				//Benchmark.Instance.Trace("TypeFinder.Instance");
				if (type != null && type != desc.Type)
				{
					desc = this.Context.Parse(desc.Name, type, outer);
				}
				//var types = TypeFinder.Instance.SearchAll(explicitType, desc.Type);
				//if (types.Count > 0)
				//{
				//    Type type = types.Find(t => t.Name != null && t.Name.ToLower() == explicitType.ToLower()) ?? desc.Type;
				//    if (type != desc.Type)
				//    {
				//        desc = this.Context.Parse(desc.Name, type, outer);
				//    }
				//}
			}
			//Benchmark.Instance.Trace();
			desc.Value = value;
			if (desc.Value == null && desc.Name != null && outer != null && outer.Value != null)
			{
				var info = outer.Property(desc.Name);
				if (info != null && info.GetIndexParameters().Length == 0)
					value = desc.Value = info.GetValue(outer.Value, null);
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
			//Benchmark.Instance.Trace("Descriptor.Generate");

			desc.Value = value;
			desc.GenerateParse();
			desc.GenerateCreate();

			//Benchmark.Instance.Trace("Descriptor.Generate");

			CWL(new StringBuilder().Append(' ', depth).Append("Deserialize(name='" + desc.Name + "', type='" + desc.Type + "', isnull='" + desc.IsNull + "', id='" + desc.Id + "', value='" + desc.Value + "', children='" + node.ChildNodes.Count + "')").ToString());
			//foreach (Descriptor cr in desc.Generated.Values) Console.WriteLine("\tid=" + cr.Id + ", name=" + cr.Name + ", type=" + cr.Type + ", value=" + cr.Value);


			List<XmlNode> nodes = new List<XmlNode>();
			if (node.Attributes != null)
				foreach (XmlNode n in node.Attributes)
					if (n.Name != AttributeType)
						nodes.Add(n);
			foreach (XmlNode n in node.ChildNodes)
				nodes.Add(n);
#if DEBUG
			if (desc.Type.Name == "Switch")
				Console.WriteLine(desc); ;//<-- break here...
#endif
			foreach (XmlNode sub in nodes)
			{
				if (sub is XmlComment)
				{
				}
				else if (sub is XmlText)
				{
					try
					{
						string text = ((XmlText)sub).Value;
						if (this.Converters.CanConvert<string>(desc.Type))
						{
							desc.Value = this.Converters.Parse<string>(desc.Type, text);
						}
						else
						{
							Descriptor inner = desc.Inner.Find(i => i.Name != null && i.Name.ToLower() == "value");
							if (inner != null)
								inner.Value = this.Converters.Parse<string>(typeof(string), text);
							else
							{
								var prop = desc.Property("Value") ?? desc.Property("Content");
								if (prop != null)
									prop.SetValue(desc.Value, this.Converters.Parse<string>(prop.PropertyType, text), null);

							}
							//desc.Property("Value").SetValue(desc.Value, this.Converters.Parse<string>(typeof(string), text), null);
						}
					}
#if CATCHALL
					catch (Exception exc)
					{
						CWL(exc);
						this.Exceptions.Add(new ExceptionTrace(exc, node, desc));
					}
#endif
					finally { }
				}
				else //if (sub is XmlElement)
				{
					try
					{
						string subTypeName = this.GetAttribute(AttributeType, sub) ?? sub.Name;
						string subName = this.GetAttribute(AttributeName, sub);
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
								subName = info.Name;
							}
							else
							{
								info = desc.Property(subTypeName, true);
								if (info != null)
								{
									subType = info.PropertyType;
									subType = TypeFinder.Instance[subTypeName, subType];
								}
								else
								{
									// try to find the type...
									subType = TypeFinder.Instance[subTypeName];
								}
							}
						}

						if (subType == null)
							continue;

						if (inner == null)
							inner = this.Context.Parse(subName, subType, desc);
						if (inner != null && !desc.Inner.Contains(inner))
							desc.Inner.Add(inner);
						if (subName != null)
							inner.Name = subName;
						inner.Value = this.Deserialize(inner.Value, inner, desc, sub);
						if (inner != null)
						{
							if (!desc.Inner.Contains(inner))
							{
								if (inner.Name != null)
								{
									desc.Inner.RemoveAll(i => i.Name == inner.Name);

									desc.Inner.Add(inner);
								}
							}
						}
					}
#if CATCHALL
					catch (Exception exc)
					{
						this.Exceptions.Add(new	ExceptionTrace(exc, node, desc));
					}
#endif
					finally { }
				}
			}
			depth--;
#if DEBUG
			if (desc.Type.Name == "Switch")
				Console.WriteLine(desc); ;//<-- break here...
#endif
			try
			{
				return desc.GenerateLink();
			}
#if CATCHALL
			catch (Exception exc)
			{
				this.Exceptions.Add(new ExceptionTrace(exc, node, desc));
				return null;
			}
#endif
			finally { }
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
		private bool? _HideUnused;

		/// <summary>
		/// Hide unused items.
		/// </summary>
		public bool HideUnused
		{
			get { return (bool)(_HideUnused ?? (_HideUnused = this.HideUnusedCreate)); }
		}

		/// <summary>
		/// Creates the default/initial value for HideUnused.
		/// Hide unused items.
		/// </summary>
		protected virtual bool HideUnusedCreate
		{
			get { return true; }
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