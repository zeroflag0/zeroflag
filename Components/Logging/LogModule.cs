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
///		<revision>$Rev: 64 $</revision>
///		<author>$Author: zeroflag $</author>
///		<id>$Id: LogModule.cs 64 2008-12-30 11:50:08Z zeroflag $</id>
///	</file>
#endregion SVN Version Information

using System;
using zeroflag.Collections;
using System.Text;

namespace zeroflag.Components.Logging
{
	public class LogModule : Module
	{
		public LogModule()
		{
			//Console.WriteLine( "LogModule created.\n" + new System.Diagnostics.StackTrace() );
		}

		#region Writers
		private List<LogWriter> _Writers;

		/// <summary>
		/// This module's writer backends.
		/// </summary>
		public List<LogWriter> Writers
		{
			get { return _Writers ?? ( _Writers = this.WritersCreate ); }
			//set { _Writers = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Writers.
		/// This module's writer backends.
		/// </summary>
		protected virtual List<LogWriter> WritersCreate
		{
			get
			{
				var Writers = _Writers = new List<LogWriter>();
				Writers.Add( new ConsoleWriter() );
				Writers.Add( new FileWriter() );
				return Writers;
			}
		}

		#endregion Writers


		#region TaskProcessor
		private zeroflag.Components.TaskProcessor _TaskProcessor;

		/// <summary>
		/// A task processor (threaded) for background logging...
		/// </summary>
		public zeroflag.Components.TaskProcessor TaskProcessor
		{
			get { return _TaskProcessor ?? ( _TaskProcessor = this.TaskProcessorCreate ); }
			//set { _TaskProcessor = value; }
		}

		/// <summary>
		/// Creates the default/initial value for TaskProcessor.
		/// A task processor (threaded) for background logging...
		/// </summary>
		protected virtual zeroflag.Components.TaskProcessor TaskProcessorCreate
		{
			get
			{
				var tp = _TaskProcessor = new zeroflag.Components.TaskProcessor() { Outer = this };
				tp.CancelTimeout = TimeSpan.FromSeconds( 1 );
				tp.Name = this.Name;
				return tp;
			}
		}

		#endregion TaskProcessor


		#region Logs
		private List<Log> _Logs;

		/// <summary>
		/// The logs managed by this module.
		/// </summary>
		public List<Log> Logs
		{
			get { return _Logs ?? ( _Logs = this.LogsCreate ); }
			//set { _Logs = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Logs.
		/// The logs managed by this module.
		/// </summary>
		protected virtual List<Log> LogsCreate
		{
			get
			{
				var Logs = _Logs = new List<Log>();
				return Logs;
			}
		}

		#endregion Logs

		protected override string NameCreate
		{
			get { return ( CoreBase ?? (object)"<corelesss>" ) + ".Log"; }
		}
		protected override void OnInitialize()
		{
			if ( this.Writers.Count <= 0 )
				// add a default writer...
				this.Writers.Add( new ConsoleWriter() );
			base.OnInitialize();
#if VERBOSE
			this.Log.Verbose( "VERBOSE logging active" );
#endif
			this.OnUpdate( TimeSpan.MinValue );
			this.TaskProcessor.Add( DateTime.Now.AddMilliseconds( 250 ), this._OnUpdate );
		}

		private int _MessagesProcessed = 0;
		protected override void OnUpdate( TimeSpan timeSinceLastUpdate )
		{
			base.OnUpdate( timeSinceLastUpdate );
		}
		protected void _OnUpdate()
		{
			//Console.WriteLine( "Log Update..." );
			//this.TaskProcessor.Add( () =>
			//    {
			// this will be used to keep the messages in timely order (because the loop below takes modules in order first, times second)
			List<LogMessage> messages = new List<LogMessage>();

			foreach ( Log log in this.Logs )
			{
				System.Collections.Generic.KeyValuePair<DateTime, string> msg;
				while ( log.Queue.First != null )
				{
					msg = log.Queue.Read();
					messages.Add( new LogMessage() { Sender = log.Owner, Time = msg.Key, Message = msg.Value } );
				}
			}
			messages.Sort();
			foreach ( var msg in messages )
				this.Write( msg.Time, msg.Sender, msg.Message );
			foreach ( LogWriter writer in this.Writers )
				writer.Flush();
			_MessagesProcessed = messages.Count;
			if ( this.State != ModuleStates.Disposed )
				this.TaskProcessor.Add( DateTime.Now.AddMilliseconds( 250 ), this._OnUpdate );
			//} );
		}

		protected void Write( DateTime time, string owner, string value )
		{
			foreach ( LogWriter writer in this.Writers )
			{
				writer.Write( time, owner, value );
			}
		}

		#region DisposeDelay
		private TimeSpan? _DisposeDelay;

		/// <summary>
		/// A delay for the log dispose. (important to get messages from other modules disposing, 5seconds by default)
		/// </summary>
		public TimeSpan DisposeDelay
		{
			get { return _DisposeDelay ?? ( _DisposeDelay = this.DisposeDelayCreate ) ?? TimeSpan.FromSeconds( 1 ); }
			set
			{
				if ( _DisposeDelay != value )
				{
					_DisposeDelay = value;
				}
			}
		}

		/// <summary>
		/// Creates the default/initial value for DisposeDelay.
		/// A delay for the log dispose. (important to get messages from other modules disposing)
		/// </summary>
		protected virtual TimeSpan? DisposeDelayCreate
		{
			get
			{
				TimeSpan? value = _DisposeDelay = TimeSpan.FromSeconds( 5 );
				return value;
			}
		}

		#endregion DisposeDelay


		protected override void OnDispose()
		{
			if ( this.CoreBase != null && this.CoreBase.State != ModuleStates.Disposed || this.Outer != null && this.Outer.State != ModuleStates.Disposed || this.TaskProcessor.Count > 1 )
			{
				this.Log.Message( "Supressing log shutdown..." );
				//this.Write( DateTime.Now, this.Name, "Waiting for core to shut down..." );
				this._OnUpdate();
				//this.TaskProcessor.Add( () => OnUpdate( TimeSpan.Zero ) );
				this.TaskProcessor.Add( DateTime.Now.Add( this.DisposeDelay ), DoDispose );
				//this.TaskProcessor.Add(
				//        () =>
				//        {
				//            if ( this._MessagesProcessed == 0 )
				//            {
				//                this.OnShutdown();
				//                this.TaskProcessor.Dispose();
				//            }
				//        } );
			}
			else
			{
				this._OnUpdate();
				this.DoDispose();
			}
		}

		protected override void OnDisposeInner()
		{
			//base.OnDisposeInner();
		}
		void DoDispose()
		{
			this._OnUpdate();
			this.Write( DateTime.Now, this.Name, "Shutdown" );
			this._OnUpdate();
			this.TaskProcessor.Finish();
			if ( this.TaskProcessor.Count > 1 )
			{
				Console.WriteLine( " *** " + this.TaskProcessor.Count + " messages left." );
				this.TaskProcessor.Add( DateTime.Now.AddSeconds( 2 ), () =>
				{
					if ( this.TaskProcessor.Count > 1 )
						Console.WriteLine( " *** " + this.TaskProcessor.Count + " messages left." );
					this.TaskProcessor.Clear();
					this.TaskProcessor.Cancel = true;
					this.TaskProcessor.Dispose();
					base.OnDispose();
					base.OnDisposeInner();
				} );
			}
			else
			{
				this.TaskProcessor.Clear();
				this.TaskProcessor.Cancel = true;
				this.TaskProcessor.Dispose();
				base.OnDispose();
				base.OnDisposeInner();
			}
		}
		//protected override void OnDisposing()
		//{
		//    this.OnUpdate( TimeSpan.Zero );
		//    base.OnDisposing();
		//    this.Write( DateTime.Now, this.Name, "Disposed" );
		//    this.OnUpdate( TimeSpan.Zero );
		//    while ( this.TaskProcessor.IsRunning )
		//        System.Threading.Thread.Sleep( 100 );
		//    this.TaskProcessor.Cancel = true;
		//    this.TaskProcessor.Dispose();
		//}

		public override int CompareTo( Module other )
		{
			if ( other == this )
				return base.CompareTo( other );
			if ( this.State == ModuleStates.Initializing )
				return 1;
			//else
			//    return -1;
			return base.CompareTo( other );
		}

		protected override void OnOuterChanged( Component oldvalue, Component newvalue )
		{
			base.OnOuterChanged( oldvalue, newvalue );
			if ( oldvalue != null && newvalue == null )
			{
				//this.Log.Message( "Disposing because parent is missing" );
				//Console.WriteLine( this.Name + " Disposing because parent is missing" );
				//this.Dispose();
			}
		}
	}
}
