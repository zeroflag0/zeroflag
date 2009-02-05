using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization
{
	public class ExceptionTrace
	{
		public ExceptionTrace( Exception exc, object node, Descriptors.Descriptor desc )
			: this( exc, node, desc, null, null )
		{
		}
		public ExceptionTrace( Exception exc, Descriptors.Descriptor desc, Type type, object value )
			: this( exc, null, desc, type, value )
		{
		}
		public ExceptionTrace( Exception exc, object node, Descriptors.Descriptor desc, Type type, object value )
		{
			this.Exception = exc;
			//this.Node = node.rea;
			this.Descriptor = desc;
			this.Type = type;
			this.Value = value;
		}

		#region Exception

		private Exception _Exception = default( Exception );
		[System.ComponentModel.TypeConverter( typeof( System.ComponentModel.ExpandableObjectConverter ) )]
		public Exception Exception
		{
			get { return _Exception; }
			set
			{
				if ( _Exception != value )
				{
					_Exception = value;
				}
			}
		}
		#endregion Exception

		#region Node

		private object _Node;
#if !SILVERLIGHT
		[System.ComponentModel.TypeConverter( typeof( System.ComponentModel.ExpandableObjectConverter ) )]
#endif
		public object Node
		{
			get { return _Node; }
			set
			{
				if ( _Node != value )
				{
					_Node = value;
				}
			}
		}
		#endregion Node

		#region Descriptor

		private Descriptors.Descriptor _Descriptor = default( Descriptors.Descriptor );

		[System.ComponentModel.TypeConverter( typeof( System.ComponentModel.ExpandableObjectConverter ) )]
		public Descriptors.Descriptor Descriptor
		{
			get { return _Descriptor; }
			set
			{
				if ( _Descriptor != value )
				{
					_Descriptor = value;
				}
			}
		}
		#endregion Descriptor

		#region Type

		private Type _Type = default( Type );

		[System.ComponentModel.TypeConverter( typeof( System.ComponentModel.ExpandableObjectConverter ) )]
		public Type Type
		{
			get { return _Type; }
			set
			{
				if ( _Type != value )
				{
					_Type = value;
				}
			}
		}
		#endregion Type

		#region Value

		private object _Value = default( object );

		[System.ComponentModel.TypeConverter( typeof( System.ComponentModel.ExpandableObjectConverter ) )]
		public object Value
		{
			get { return _Value; }
			set
			{
				if ( _Value != value )
				{
					_Value = value;
				}
			}
		}
		#endregion Value

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( "ExceptionTrace: " );
			try
			{
				if ( this.Exception != null )
					sb.Append( this.Exception.Message ).Append( "\n\t" )
						.Append( "Stacktrace:\n\t\t" )
						.Append( this.Exception.StackTrace.Replace( "\n", "\n\t" ) ).Append( "\n\t" );
				else
				{
					sb.AppendLine( "<trace>\n\t" );
				}
				sb.Append( "Descriptor: " ).Append( this.Descriptor ).Append( "\n\t" );
				if ( this.Descriptor != null && this.Descriptor.Property != null )
				{
					sb.Append( "Property: " ).Append( this.Descriptor.Property ).Append( "\n\t" );
				}
				sb.Append( "Value: " ).Append( this.Value ?? (object)"<null>" ).Append( "\n\t" );
				sb.Append( "Type: " ).Append( this.Type ?? (object)"<null>" ).Append( "\n\t" );
				//if ( Node != null && Node.OuterXml != null )
				//    sb.Append( "Xml:\n\t\t" ).Append( this.Node.OuterXml.Replace( "\n", "\n\t" ) ).Append( "\n" );
			}
			catch ( Exception exc )
			{
				sb.Append( exc );
			}
			return sb.ToString();
		}
	}
}
