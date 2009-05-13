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
///		<revision>$Rev: 29 $</revision>
///		<author>$Author: zeroflag $</author>
///		<id>$Id: ConsoleWriter.cs 29 2008-09-24 06:56:58Z zeroflag $</id>
///	</file>
#endregion SVN Version Information

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Components.Logging
{
	public class MemoryWriter : LogWriter
	{
		#region Buffer
		private zeroflag.Threading.LocklessQueue<string> _Buffer;

		/// <summary>
		/// Buffer for the log.
		/// </summary>
		public zeroflag.Threading.LocklessQueue<string> Buffer
		{
			get { return _Buffer ?? ( _Buffer = this.BufferCreate ); }
			//protected set
			//{
			//	if (_Buffer != value)
			//	{
			//		//if (_Buffer != null) { }
			//		_Buffer = value;
			//		//if (_Buffer != null) { }
			//	}
			//}
		}

		/// <summary>
		/// Creates the default/initial value for Buffer.
		/// Buffer for the log.
		/// </summary>
		protected virtual zeroflag.Threading.LocklessQueue<string> BufferCreate
		{
			get
			{
				zeroflag.Threading.LocklessQueue<string> value = null;
				lock ( this )
				{
					if ( _Buffer == null )
					{
						value = _Buffer = new zeroflag.Threading.LocklessQueue<string>();
					}
				}
				return value ?? _Buffer;
			}
		}

		#endregion Buffer

		#region event NewMessage
		public delegate void NewMessageHandler( string line );

		private event NewMessageHandler _NewMessage;
		/// <summary>
		/// When a new message was written.
		/// </summary>
		public event NewMessageHandler NewMessage
		{
			add { this._NewMessage += value; }
			remove { this._NewMessage -= value; }
		}
		/// <summary>
		/// Call to raise the NewMessage event:
		/// When a new message was written.
		/// </summary>
		protected virtual void OnNewMessage( string line )
		{
			// if there are event subscribers...
			if ( this._NewMessage != null )
			{
				// call them...
				this._NewMessage( line );
			}
		}
		#endregion event NewMessage

		public override void Write( DateTime time, string owner, string text )
		{
			while ( this.Buffer.Count > 1024 )
				this.Buffer.Read();
			string msg = new StringBuilder( time.ToString( "HH:mm:ss.fff" ) ).Append( " [" ).Append( owner.PadRight( 15 ) ).Append( "] " ).Append( text ).ToString();
			this.Buffer.Write( msg );
			this.OnNewMessage( msg );
		}

		public override void Flush()
		{
		}


		public override void Dispose()
		{
			this.Write( DateTime.Now, "Memory", "closed" );
		}
	}
}
