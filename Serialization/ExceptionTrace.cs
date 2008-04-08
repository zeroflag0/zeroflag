using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization
{
	public class ExceptionTrace
	{
		public ExceptionTrace(Exception exc, System.Xml.XmlNode node, Descriptors.Descriptor desc)
			: this(exc, node, desc, null, null)
		{
		}
		public ExceptionTrace(Exception exc, Descriptors.Descriptor desc, Type type, object value)
			: this(exc, null, desc, type, value)
		{
		}
		public ExceptionTrace(Exception exc, System.Xml.XmlNode node, Descriptors.Descriptor desc, Type type, object value)
		{
			this.Exception = exc;
			this.Node = node;
			this.Descriptor = desc;
			this.Type = type;
			this.Value = value;
		}

		#region Exception

		private Exception _Exception = default(Exception);
		[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
		public Exception Exception
		{
			get { return _Exception; }
			set
			{
				if (_Exception != value)
				{
					_Exception = value;
				}
			}
		}
		#endregion Exception

		#region Node

		private System.Xml.XmlNode _Node = default(System.Xml.XmlNode);

		[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
		public System.Xml.XmlNode Node
		{
			get { return _Node; }
			set
			{
				if (_Node != value)
				{
					_Node = value;
				}
			}
		}
		#endregion Node

		#region Descriptor

		private Descriptors.Descriptor _Descriptor = default(Descriptors.Descriptor);

		[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
		public Descriptors.Descriptor Descriptor
		{
			get { return _Descriptor; }
			set
			{
				if (_Descriptor != value)
				{
					_Descriptor = value;
				}
			}
		}
		#endregion Descriptor

		#region Type

		private Type _Type = default(Type);

		[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
		public Type Type
		{
			get { return _Type; }
			set
			{
				if (_Type != value)
				{
					_Type = value;
				}
			}
		}
		#endregion Type

		#region Value

		private object _Value = default(object);

		[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
		public object Value
		{
			get { return _Value; }
			set
			{
				if (_Value != value)
				{
					_Value = value;
				}
			}
		}
		#endregion Value
	}
}
