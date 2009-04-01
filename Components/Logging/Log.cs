#region BSD license
/*
 * Copyright (c) 2008, Thomas "zeroflag" Kraemer. All rights reserved.
 * Copyright (c) 2008, Anders "anonimasu" Helin. All rights reserved.
 * Copyright (c) 2008, The zeroflag.Components.NET Team. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, 
 * this list of conditions and the following disclaimer in the documentation 
 * and/or other materials provided with the distribution.
 * Neither the name of the zeroflag.Components.NET Team nor the names of its contributors may 
 * be used to endorse or promote products derived from this software without 
 * specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion BSD license

#region SVN Version Information
///	<file>
///		<!-- Last modification of this file: -->
///		<revision>$Rev: 45 $</revision>
///		<author>$Author: zeroflag $</author>
///		<id>$Id: Log.cs 45 2008-10-30 12:10:08Z zeroflag $</id>
///	</file>
#endregion SVN Version Information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Components.Logging
{
	public class Log
	{
		public Log()
			: this( "" )
		{

		}

		public Log( string name )
			: this( null, name )
		{
		}

		public Log( LogModule module, string name )
		{
			this.Module = module;
			this.Owner = name;
		}


		#region Queue
		private zeroflag.Threading.LocklessQueue<KeyValuePair<DateTime, string>> _Queue;

		/// <summary>
		/// This log's message-queue...
		/// </summary>
		public zeroflag.Threading.LocklessQueue<KeyValuePair<DateTime, string>> Queue
		{
			get { return _Queue ?? ( _Queue = this.QueueCreate ); }
			//set { _Queue = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Queue.
		/// This log's message-queue...
		/// </summary>
		protected virtual zeroflag.Threading.LocklessQueue<KeyValuePair<DateTime, string>> QueueCreate
		{
			get
			{
				var Queue = _Queue = new zeroflag.Threading.LocklessQueue<KeyValuePair<DateTime, string>>();

				return Queue;
			}
		}

		#endregion Queue

		LogModule _Module;

		public LogModule Module
		{
			get { return _Module; }
			set
			{
				if ( _Module != value )
				{
					_Module = value;
					if ( value != null )
						value.Logs.Add( this );
				}
			}
		}

		string _Owner;

		public virtual string Owner
		{
			get { return _Owner; }
			set { _Owner = value; }
		}

		bool _Quiet = false;
		/// <summary>
		/// If quiet is true, messages won't be logged. Has no effect on errors and warnings.
		/// </summary>
		public bool Quiet
		{
			get { return _Quiet; }
			set { _Quiet = value; }
		}

		#region Indent
		string _Indent = "\t";

		public virtual string Indent
		{
			get { return _Indent; }
			set { _Indent = value; }
		}

		protected virtual string CreateIndent( int count )
		{
			StringBuilder builder = new StringBuilder(/*this.Indent.Length * count*/);

			for ( int i = 0; i < count; i++ )
			{
				builder.Append( this.Indent );
				//for(int j = 0; j < this.Indent.Length; j++)
				//{
				//    builder[i + j] = this.Indent[j];
				//}
			}

			return builder.ToString();
		}
		#endregion Indent

		//protected virtual void DoWrite( DateTime time, string value )
		//{
		//    if ( this.Module != null )
		//        this.Module.Write( time, this.Owner, value );
		//    else
		//        return;
		//}
		static Func<string> empty = () => "";

		#region Message
		public virtual void Message( object value )
		{
			if ( !this.Quiet )
			{
				if ( value == null )
				{
					this.Message( "<null>" );
				}
				else
				{
					this.Message( value.ToString() );
				}
			}
		}

		string _MessageIndentBuffer = null;
		int _MessageIndentBufferCount = -1;

		protected virtual string MessageIndentBuffer
		{
			get
			{
				if ( this._MessageIndentBuffer == null || this._MessageIndentBufferCount != this.MessageIndent )
				{
					this._MessageIndentBuffer = this.CreateIndent( this.MessageIndent );
				}
				return _MessageIndentBuffer;
			}
		}

		public virtual void Message( string value )
		{
			if ( !this.Quiet )
			{
				this.WriteMessage( Now, new StringBuilder( this.MessageIndentBuffer ).Append( ( this.MessagePrefix ?? empty )() ).Append( value ).Append( ( this.MessagePostfix ?? empty )() ).ToString() );
			}
		}

		protected virtual void WriteMessage( DateTime time, string value )
		{
#if LOG_DIRECT || LOG_DIRECT_MESSAGE
			System.Console.WriteLine( new StringBuilder( time.ToString( "HH:mm:ss.fff" ) ).Append( " [" ).Append( this.Owner.PadRight( 15 ) ).Append( "]** " ).Append( value ) );
#endif
			this.Queue.Add( new KeyValuePair<DateTime, string>( time, value ) );
		}

		int _MessageIndent = 0;

		public virtual int MessageIndent
		{
			get { return _MessageIndent; }
			set { _MessageIndent = value; }
		}

		Func<string> _MessagePrefix;

		public virtual Func<string> MessagePrefix
		{
			get { return _MessagePrefix; }
			set { _MessagePrefix = value; }
		}

		Func<string> _MessagePostfix;

		public virtual Func<string> MessagePostfix
		{
			get { return _MessagePostfix; }
			set { _MessagePostfix = value; }
		}

		#endregion Message

		#region Warning

		public virtual void Warning( object value )
		{
			if ( value == null )
			{
				this.Warning( "<null>" );
			}
			else
			{
				this.Warning( value.ToString() );
			}
		}

		string _WarningIndentBuffer = null;
		int _WarningIndentBufferCount = -1;

		protected virtual string WarningIndentBuffer
		{
			get
			{
				if ( this._WarningIndentBuffer == null || this._WarningIndentBufferCount != this.WarningIndent )
				{
					this._WarningIndentBuffer = this.CreateIndent( this.WarningIndent );
				}
				return _WarningIndentBuffer;
			}
		}

		public virtual void Warning( string value )
		{
			this.WriteWarning( Now, new StringBuilder( this.WarningIndentBuffer ).Append( ( this.WarningPrefix ?? empty )() ).Append( value ).Append( ( this.WarningPostfix ?? empty )() ).ToString() );
		}

		protected virtual void WriteWarning( DateTime time, string value )
		{
#if LOG_DIRECT || LOG_DIRECT_WARNING
			System.Console.WriteLine( new StringBuilder( time.ToString( "HH:mm:ss.fff" ) ).Append( " [" ).Append( this.Owner.PadRight( 15 ) ).Append( "]** " ).Append( value ) );
#endif
			this.Queue.Add( new KeyValuePair<DateTime, string>( time, value ) );
		}


		int _WarningIndent = 0;

		public virtual int WarningIndent
		{
			get { return _WarningIndent; }
			set { _WarningIndent = value; }
		}

		Func<string> _WarningPrefix = () => "[Warning] ";

		public virtual Func<string> WarningPrefix
		{
			get { return _WarningPrefix; }
			set { _WarningPrefix = value; }
		}

		Func<string> _WarningPostfix;

		public Func<string> WarningPostfix
		{
			get { return _WarningPostfix; }
			set { _WarningPostfix = value; }
		}

		#endregion Warning

		#region Error

		public virtual void Error( object value )
		{
			if ( value == null )
			{
				this.Error( "<null>" );
			}
			else
			{
				this.Error( value.ToString() );
			}
		}

		string _ErrorIndentBuffer = null;
		int _ErrorIndentBufferCount = -1;

		protected virtual string ErrorIndentBuffer
		{
			get
			{
				if ( this._ErrorIndentBuffer == null || this._ErrorIndentBufferCount != this.ErrorIndent )
				{
					this._ErrorIndentBuffer = this.CreateIndent( this.ErrorIndent );
				}
				return _ErrorIndentBuffer;
			}
		}

		public virtual void Error( string value )
		{
			this.WriteError( Now, new StringBuilder( this.ErrorIndentBuffer ).Append( ( this.ErrorPrefix ?? empty )() ).Append( value ).Append( ( this.ErrorPostfix ?? empty )() ).ToString() );
		}

		protected virtual void WriteError( DateTime time, string value )
		{
#if LOG_DIRECT || LOG_DIRECT_ERROR
			System.Console.WriteLine( new StringBuilder( time.ToString( "HH:mm:ss.fff" ) ).Append( " [" ).Append( this.Owner.PadRight( 15 ) ).Append( "]** " ).Append( value ) );
#endif
			this.Queue.Add( new KeyValuePair<DateTime, string>( time, value ) );
		}

		int _ErrorIndent = 0;

		public int ErrorIndent
		{
			get { return _ErrorIndent; }
			set { _ErrorIndent = value; }
		}

		Func<string> _ErrorPrefix = () => "[ERROR] ";

		public Func<string> ErrorPrefix
		{
			get { return _ErrorPrefix; }
			set { _ErrorPrefix = value; }
		}


		Func<string> _ErrorPostfix;

		public Func<string> ErrorPostfix
		{
			get { return _ErrorPostfix; }
			set { _ErrorPostfix = value; }
		}

		#endregion Error

		#region Verbose
		[System.Diagnostics.Conditional( "VERBOSE" )]
		public virtual void Verbose( object value )
		{
			if ( !this.Quiet )
			{
				if ( value == null )
				{
					this.Verbose( "<null>" );
				}
				else
				{
					this.Verbose( value.ToString() );
				}
			}
		}

		string _VerboseIndentBuffer = null;
		int _VerboseIndentBufferCount = -1;

		protected virtual string VerboseIndentBuffer
		{
			get
			{
				if ( this._VerboseIndentBuffer == null || this._VerboseIndentBufferCount != this.VerboseIndent )
				{
					this._VerboseIndentBuffer = this.CreateIndent( this.VerboseIndent );
				}
				return _VerboseIndentBuffer;
			}
		}
		[System.Diagnostics.Conditional( "VERBOSE" )]
		public virtual void Verbose( string value )
		{
			if ( !this.Quiet )
			{
				this.WriteVerbose( Now, new StringBuilder( this.VerboseIndentBuffer ).Append( ( this.VerbosePrefix ?? empty )() ).Append( value ).Append( ( this.VerbosePostfix ?? empty )() ).ToString() );
			}
		}
		[System.Diagnostics.Conditional( "VERBOSE" )]
		protected virtual void WriteVerbose( DateTime time, string value )
		{
#if LOG_DIRECT
			System.Console.WriteLine( new StringBuilder( time.ToString( "HH:mm:ss.fff" ) ).Append( " [" ).Append( this.Owner.PadRight( 15 ) ).Append( "]** " ).Append( value ) );
#endif
			this.Queue.Add( new KeyValuePair<DateTime, string>( time, value ) );
		}

		int _VerboseIndent = 0;

		public virtual int VerboseIndent
		{
			get { return _VerboseIndent; }
			set { _VerboseIndent = value; }
		}

		Func<string> _VerbosePrefix;

		public virtual Func<string> VerbosePrefix
		{
			get { return _VerbosePrefix; }
			set { _VerbosePrefix = value; }
		}

		Func<string> _VerbosePostfix;

		public virtual Func<string> VerbosePostfix
		{
			get { return _VerbosePostfix; }
			set { _VerbosePostfix = value; }
		}

		#endregion Verbose

		public static implicit operator Action<object>( Log log )
		{
			if ( log != null )
				return log.Message;
			else
				return null;
		}

		DateTime _Now;
		int _NowCount;
		public DateTime Now
		{
			get
			{
				DateTime now = DateTime.Now;
				if ( now == _Now )
				{
					_NowCount++;
					now.AddMilliseconds( 0.0001 * _NowCount );
					return now;
				}
				else
				{
					_Now = now;
					_NowCount = 0;
					return now;
				}
			}
		}
	}
}
