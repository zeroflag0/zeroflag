//#define DEBUGTYPE
using System;
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
		static readonly List<string> Attributes = new List<string>()
		{
			AttributeName,
			AttributeType,
			AttributeNull,
			AttributeId,
		};

		#region Serialize

		#region XslStyleSheet

		private string _XslStyleSheet;

		/// <summary>
		/// Path to a file that should be refered to from the generated xml files.
		/// </summary>
		public string XslStyleSheet
		{
			get { return _XslStyleSheet; }
			set
			{
				if ( _XslStyleSheet != value )
				{
					_XslStyleSheet = value;
				}
			}
		}

		#endregion XslStyleSheet


		public override void Serialize( zeroflag.Serialization.Descriptors.Descriptor value )
		{
			using ( System.IO.StreamWriter writer =
				new System.IO.StreamWriter(
					this.Stream != null ?
					this.Stream :
					System.IO.File.Open( this.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read )
					)
				)
			{
				using ( XmlTextWriter doc = new XmlTextWriter( writer ) )
				{
					doc.Formatting = Formatting.Indented;
					//if ( this.FileName != null )
					//    doc = new XmlTextWriter( new System.IO.StreamWriter( this.FileName ) );
					//else if ( this.Stream != null )
					//    doc = new XmlTextWriter( new System.IO.StreamWriter( this.Stream ) );

					//XmlDocument doc = new XmlDocument();
					doc.WriteStartDocument();
					//doc.AppendChild( doc.CreateXmlDeclaration( "1.0", null, null ) );
					if ( this.XslStyleSheet != null )
						doc.WriteProcessingInstruction( "xml-stylesheet", "type=\"text/xsl\" href=\"" + this.XslStyleSheet + "\"" );
					//doc.AppendChild( doc.CreateProcessingInstruction( "xml-stylesheet", "type=\"text/xsl\" href=\"" + this.XslStyleSheet + "\"" ) );
					//doc.AppendChild(doc.CreateComment(value.ToStringTree().ToString()));
					//doc.AppendChild(doc.CreateElement("root"));

					this.Serialize( value, doc, null, new List<int>() );
					//if ( this.FileName != null )
					//    doc.Save( this.FileName );
					//else if ( this.Stream != null )
					//    doc.Save( this.Stream );
				}
			}
		}

		protected virtual void Serialize( zeroflag.Serialization.Descriptors.Descriptor desc, XmlTextWriter doc, zeroflag.Serialization.Descriptors.Descriptor valueParent, List<int> ids )
		{
			if ( desc == null || desc.IsNull )
				return;

			if ( desc.Value != null )
				base.OnProgressItem( desc.Value );

			//desc.Inner.Clear();
			//desc.Parse(desc.Name, desc.Type, desc.Value);
			bool explicitType = false;
			if ( desc.Value != null && desc.Value.GetType() != desc.Type )
			{
				desc.Type = desc.Value.GetType();
				//explicitType = true;
			}

			//if ( this.IgnoreList.Find( i => i != null && i( desc ) ) != null )
			//    return;

			string name = desc.Name ?? desc.Type.Name.Split( '`' )[0];
			CWL( "Serialize(" + desc + ")" );
			if ( this.Converters.CanConvert<string>( desc.Value ) )
			{
				if ( desc.Name == null )
				{
					if ( !( desc.IsNull && this.HideUnused ) )
					{

						doc.WriteStartElement( name );
						try
						{
							//XmlElement node = doc.CreateElement( name );

							if ( valueParent != null && valueParent.Value != null && desc.Name != null )
							{
								var info = valueParent.Type.GetProperty( desc.Name );
								if ( info != null )
								{
									explicitType = info.PropertyType != desc.Type;
								}
							}

							if ( explicitType )
								doc.WriteAttributeString( AttributeType, desc.Type.FullName ?? "" );

							//this.WriteAttribute( AttributeType, desc.Type.FullName, doc );

							doc.WriteValue( this.Converters.Generate<string>( desc.Type, desc.Value ) );
						}
						finally
						{
							doc.WriteEndElement();
						}
						//node.InnerText = this.Converters.Generate<string>( desc.Type, desc.Value );

						//if ( !( desc.IsNull && this.HideUnused ) )
						//{
						//    if ( xmlParent != null )
						//        xmlParent.AppendChild( node );
						//    else
						//        doc.AppendChild( node );
					}
				}
				else
				{
					//XmlAttribute node = doc.CreateAttribute( desc.Name );
					//node.Value = this.Converters.Generate<string>( desc.Type, desc.Value );

					//this.WriteAttribute("value", StringConverters.Base.Write(value.Value), doc, node);

					if ( !( desc.IsNull && this.HideUnused ) )
					{
						doc.WriteAttributeString( desc.Name, this.Converters.Generate<string>( desc.Type, desc.Value ) );
						//if ( xmlParent != null )
						//    xmlParent.Attributes.Append( node );
						//else
						//    doc.Attributes.Append( node );
					}
				}
			}
			else if ( this.SimplifyOutput && desc.Value != null && ( desc.Type.GetMethod( "Parse", System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod ) != null ) )
			{
				//XmlAttribute node = doc.CreateAttribute( name );
				//node.Value = desc.Value.ToString();

				if ( !( desc.IsNull && this.HideUnused ) )
				{
					doc.WriteAttributeString( name, desc.Value.ToString() );
					//if ( xmlParent != null )
					//    xmlParent.Attributes.Append( node );
					//else
					//    doc.Attributes.Append( node );
				}
			}
			else // || value.Name == null)
			{
				// complex type...
				if ( !( desc.IsNull && this.HideUnused ) )
				{
					//XmlElement node = doc.CreateElement( name );
					doc.WriteStartElement( name );
					try
					{
						if ( valueParent != null && valueParent.Value != null && desc.Name != null )
						{
							var info = valueParent.Type.GetProperty( desc.Name );
							if ( info != null )
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

						//if ( xmlParent != null )
						//    xmlParent.AppendChild( node );
						//else
						//    doc.AppendChild( node );


						bool hasId = desc.Id > -1 && desc.IsReferenced;
						bool isReference = false;
						if ( desc.Id > -1 && desc.IsReferenced )
						{
							//this.WriteAttribute( AttributeId, desc.Id.ToString(), doc, node );
							doc.WriteAttributeString( AttributeId, desc.Id.ToString() );
							if ( ids.Contains( desc.Id ?? -1 ) )
								isReference = true;
							else
								ids.Add( desc.Id ?? -1 );
						}

						if ( !isReference )
						{
							// serialize inner...
							List<Descriptor> complex = new List<Descriptor>();
							foreach ( Descriptor inner in desc.Inner )
							{
								if ( this.HideUnused && inner.Name != null )
								{
									if ( !desc.Property( inner.Name ).CanWrite && inner.NeedsWriteAccess )
										continue;
									if ( inner is IListDescriptor && inner.Value != null &&
										( ( inner.Value is System.Collections.ICollection && ( (System.Collections.ICollection)inner.Value ).Count <= 0 )
										) )
										continue;
								}
								if ( this.IgnoreList.Find( i => i != null && i( inner ) ) != null )
									continue;
								if ( !( this.SimplifyOutput && inner.Value != null && ( inner.Type.GetMethod( "Parse", System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod, null, System.Reflection.CallingConventions.Any, new Type[] { typeof( string ) }, new System.Reflection.ParameterModifier[0] ) != null ) ) &&
									( inner.Name == null || !this.Converters.CanConvert<string>( inner.Value ) ) )
								{
									// complex type...
									complex.Add( inner );
								}
								else
								{
									// simple type (attribute)...
									this.Serialize( inner, doc, desc, ids );
								}
							}
							if ( explicitType )
								doc.WriteAttributeString( AttributeType, desc.Type.FullName );
							//this.WriteAttribute( AttributeType, desc.Type.FullName, doc, node );

							//if (value.IsNull)
							//    this.WriteAttribute(AttributeNull, value.IsNull.ToString(), doc, node);

							//#if DEBUG
							//            this.WriteAttribute("descriptor", value.ToString(), doc, node);
							//#endif

							foreach ( Descriptor inner in complex )
								this.Serialize( inner, doc, desc, ids );
						}
					}
					finally
					{
						doc.WriteEndElement();
					}
				}
			}
		}

		//protected void WriteAttribute( string name, string value, XmlTextWriter doc )
		//{
		//    doc.WriteAttributeString( name, value ?? "" );
		//    //XmlAttribute att = doc.CreateAttribute( name );
		//    //att.Value = value ?? "";
		//    //parent.Attributes.Append( att );
		//    //return parent;
		//}

		#endregion Serialize

		#region Deserialize

		public override object Deserialize( object value, zeroflag.Serialization.Descriptors.Descriptor desc )
		{
			using ( System.IO.StreamReader reader =
				new System.IO.StreamReader(
					this.Stream != null ?
					this.Stream :
					System.IO.File.Open( this.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite )
					)
				)
			{
				using ( XmlTextReader doc = new XmlTextReader( reader ) )
				{

					//XmlDocument doc = new XmlDocument();
					//doc.Load( this.FileName );
					doc.MoveToContent();
					//doc.Read();

					value = this.Deserialize( value, desc, null, doc );

					//Console.WriteLine("<Deserialized>");
					//Console.WriteLine(desc.ToStringTree().ToString());
					//Console.WriteLine("</Deserialized>");
					//Console.WriteLine("<Created>");
					//Console.WriteLine(this.Context.Parse(value).ToStringTree().ToString());
					//Console.WriteLine("</Created>");

					return value;
				}
			}
		}
#if DEBUGTYPE
		const string BreakOnType = "";
#endif
		int depth = 0;
		protected virtual object Deserialize( object value, Descriptor desc, Descriptor outer, XmlReader node )
		{
			depth++;
			//Benchmark.Instance.Trace("Deserialize", desc, node);
#if DEBUGTYPE
			if ( desc.Type.Name == BreakOnType )
				Console.WriteLine( desc ); ;//<-- break here...
#endif
			//string explicitType = this.GetAttribute( AttributeType, node );
			string explicitType = node.GetAttribute( AttributeType );
			if ( desc.Name == null )
			{
				//desc.Name = this.GetAttribute(AttributeName, node) ?? node.Name;


				explicitType = explicitType ?? node.Name;
			}

			if ( explicitType != null )
			{
				//Benchmark.Instance.Trace("TypeFinder.Instance"); 
				Type type = TypeFinder.Instance[explicitType, desc.Type];
				//Benchmark.Instance.Trace("TypeFinder.Instance");
				if ( type != null && type != desc.Type )
				{
					desc = this.Context.Parse( desc.Name, type, outer );
				}
				else
					desc.Type = type;
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
			if ( desc.Value == null && desc.Name != null && outer != null && outer.Value != null )
			{
				var info = outer.Property( desc.Name );
				if ( info != null && info.GetIndexParameters().Length == 0 )
				{
					value = desc.Value = info.GetValue( outer.Value, null );
				}
			}
			if ( value != null && value.GetType() != desc.Type && !desc.Type.IsAssignableFrom( value.GetType() ) )
				value = null;

			if ( desc.Type.IsValueType )
			{
				desc.IsNull = false;
			}
			else
			{
				var attnull = this.GetAttribute( AttributeNull, node );
				if ( attnull != null )
					desc.IsNull = false;
				else
				{
					bool isnull = false;
					bool.TryParse( attnull, out isnull );
					desc.IsNull = isnull;
				}
				if ( !desc.IsNull )
				{
					var attid = this.GetAttribute( AttributeId, node );
					if ( attid != null )
					{
						int id;
						int.TryParse( attid, out id );
						desc.Id = id;
					}
				}
			}
			//Benchmark.Instance.Trace("Descriptor.Generate");

			desc.Value = value;
			desc.GenerateParse();
			desc.GenerateCreate();

			//Benchmark.Instance.Trace("Descriptor.Generate");

			CWL( new StringBuilder().Append( ' ', depth ).Append( "Deserialize(name='" + desc.Name + "', type='" + desc.Type + "', isnull='" + desc.IsNull + "', id='" + desc.Id + "', value='" + desc.Value + /*"', children='" + node.ChildNodes.Count + */"')" ).ToString() );
			//foreach (Descriptor cr in desc.Generated.Values) Console.WriteLine("\tid=" + cr.Id + ", name=" + cr.Name + ", type=" + cr.Type + ", value=" + cr.Value);


			//List<XmlNode> nodes = new List<XmlNode>();
			//if ( node.Attributes != null )
			//    foreach ( XmlNode n in node.Attributes )
			//        if ( n.Name != AttributeType )
			//            nodes.Add( n );
			//foreach ( XmlNode n in node.ChildNodes )
			//    nodes.Add( n );

			bool iselement = false;
			XmlReader tree = node;
			if ( node.NodeType == XmlNodeType.Element )// && !node.IsEmptyElement )
			{
				iselement = true;
				tree = node.ReadSubtree();
				tree.Read();
			}
			foreach ( XmlReader sub in this.ReadElements( tree ) )
			//while ( sub.Read() )
			{
				//if ( typeof( XmlComment ).IsAssignableFrom( sub.NodeType ) )
				//if ( sub.NodeType == XmlNodeType.Comment || sub.NodeType == XmlNodeType.Whitespace || )
				//{
				//}
				//else if ( typeof( XmlText ).IsAssignableFrom( sub.NodeType ) )
				//else 
				if ( sub.NodeType == XmlNodeType.Text || sub.NodeType == XmlNodeType.Attribute && !iselement )
				{
					try
					{
						string text = null;
						if ( sub.NodeType == XmlNodeType.Attribute )
							text = sub.Value;
						else if ( sub.NodeType == XmlNodeType.Text )
							text = sub.ReadString();//( (XmlText)sub ).Value;

						if ( this.Converters.CanConvert<string>( desc.Type ) )
						{
							desc.Value = this.Converters.Parse<string>( desc.Type, text );
						}
						else
						{
							Descriptor inner = desc.Inner.Find( i => i.Name != null && i.Name.ToLower() == "value" );
							if ( inner != null )
								inner.Value = this.Converters.Parse<string>( typeof( string ), text );
							else
							{
								var prop = desc.Property( "Value" ) ?? desc.Property( "Content" );
								if ( prop != null )
									prop.SetValue( desc.Value, this.Converters.Parse<string>( prop.PropertyType, text ), null );
								else
								{
									var meth = desc.Type.GetMethod( "Parse", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.InvokeMethod );
									if ( meth != null )
									{
										desc.Value = meth.Invoke( null, new object[] { text } );
									}
									else
									{
										desc.Value = text;
									}
								}
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
				else if ( sub.NodeType == XmlNodeType.Element || ( sub.NodeType == XmlNodeType.Text || sub.NodeType == XmlNodeType.Attribute ) && iselement )
				//if (sub is XmlElement)
				{
					try
					{
						string subTypeName = this.GetAttribute( AttributeType, sub ) ?? sub.Name;
						string subName = this.GetAttribute( AttributeName, sub );
						Type subType = null;
						Descriptor inner = null;
						inner = desc.Inner.Find( i => i.Name != null && i.Name.ToLower() == sub.Name.ToLower() );
						// try to find the property in the parent descriptor...
						if ( inner != null )
							subType = inner.Type;
						else
						{
							// try to find the property on the type...
							//var info = new List<System.Reflection.PropertyInfo>(desc.Type.GetProperties()).Find(i => i.Name.ToLower() == sub.Name.ToLower());
							var info = desc.Property( subTypeName );
							if ( info != null )
							{
								subType = info.PropertyType;
								subName = info.Name;
							}
							else if ( desc is IListDescriptor )
							{
								subType = ( (IListDescriptor)desc ).ItemType;
								if ( subTypeName != null )
									subType = TypeFinder.Instance[subTypeName, subType] ?? subType;
							}
							else
							{
								info = desc.Property( subTypeName, true );
								if ( info != null )
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

						if ( subType == null )
							continue;

						if ( !desc.Parsed )
						{
							CWL( "Parse was previously suspended on " + desc + "..." );
							//this.Context.Parse( desc );
						}

						if ( inner == null )
							inner = this.Context.Parse( subName, subType, desc );
						if ( inner != null && !desc.Inner.Contains( inner ) )
							desc.Inner.Add( inner );
						if ( inner == null )
						{
							this.Exceptions.Add( new ExceptionTrace( new Exception( "Cannot find property: " + subName + ", " + subType + ", " + subTypeName ), null, desc, desc.Type, desc.Value ) );
							CWL( "Cannot find property: " + subName + ", " + subType + ", " + subTypeName );
							continue;
						}

						if ( subName != null )
							inner.Name = subName;
						inner.Value = this.Deserialize( inner.Value, inner, desc, sub );
						if ( inner != null )
						{
							if ( !desc.Inner.Contains( inner ) )
							{
								if ( inner.Name != null )
								{
									desc.Inner.RemoveAll( i => i.Name == inner.Name );

									desc.Inner.Add( inner );
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
#if DEBUGTYPE
			if ( desc.Type.Name == BreakOnType )
				Console.WriteLine( desc );//<-- break here...
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

		IEnumerable<XmlReader> ReadElements( XmlReader node )
		{
			if ( node.NodeType == XmlNodeType.Element )
			{
				while ( node.MoveToNextAttribute() )
				{
					if ( !Attributes.Contains( node.Name ) )
						yield return node;
				}
				//for ( int i = 0; i < node.AttributeCount; i++ )
				//{
				//    node.MoveToAttribute( i );
				//    if ( !Attributes.Contains( node.Name ) )
				//        yield return node;
				//}
				node.MoveToContent();
				while ( node.Read() )
					yield return node;
			}
			else
			{
				yield return node;
			}
		}

		string GetAttribute( string name, XmlReader node )
		{
			try
			{
				//return node.Attributes[name].Value;
				return node.GetAttribute( name );
			}
			catch ( Exception exc1 )
			{
				//try
				//{
				//    //foreach ( XmlAttribute att in node.Attributes )
				//    //{
				//    //    if ( att.Name != null && att.Name.ToLower() == name.ToLower() )
				//    //        return att.Value;
				//    //}
				//    //Console.WriteLine(exc1);
				//    return null;
				//}
				//catch ( Exception exc )
				{
					//Console.WriteLine(exc);
					return null;
				}
			}
		}
		#endregion Deserialize



		#region IgnoreList
		private zeroflag.Collections.List<Predicate<Descriptor>> _IgnoreList;

		/// <summary>
		/// A list of rules to ignore certain items during serialization...
		/// </summary>
		public zeroflag.Collections.List<Predicate<Descriptor>> IgnoreList
		{
			get { return _IgnoreList ?? ( _IgnoreList = this.IgnoreListCreate ); }
			//set { _IgnoreList = value; }
		}

		/// <summary>
		/// Creates the default/initial value for IgnoreList.
		/// A list of rules to ignore certain items during serialization...
		/// </summary>
		protected virtual zeroflag.Collections.List<Predicate<Descriptor>> IgnoreListCreate
		{
			get { return new zeroflag.Collections.List<Predicate<Descriptor>>(); }
		}

		#endregion IgnoreList



		#region SimplifyOutput
		private bool? _SimplifyOutput;

		/// <summary>
		/// Simplify output? (may cause problems)
		/// </summary>
		public bool SimplifyOutput
		{
			get { return (bool)( _SimplifyOutput ?? ( _SimplifyOutput = this.SimplifyOutputCreate ) ); }
			set { _SimplifyOutput = value; }
		}

		/// <summary>
		/// Creates the default/initial value for SimplifyOutput.
		/// Simplify output? (may cause problems)
		/// </summary>
		protected virtual bool SimplifyOutputCreate
		{
			get { return true; }
		}

		#endregion SimplifyOutput


		#region HideUnused
		private bool? _HideUnused;

		/// <summary>
		/// Hide unused items.
		/// </summary>
		public bool HideUnused
		{
			get { return (bool)( _HideUnused ?? ( _HideUnused = this.HideUnusedCreate ) ); }
			set { _HideUnused = value; }
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

		public ZmlSerializer( string fileName )
			: base( fileName )
		{
		}

		public ZmlSerializer( ZmlSerializer parent )
			: base( parent )
		{
		}

		[System.Diagnostics.Conditional( "VERBOSE" )]
		static internal void CWL( object value )
		{
			Console.WriteLine( value );
		}
	}
}
