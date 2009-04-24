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
	public class PerformanceLog : zeroflag.Components.Component
	{

		#region Log

		private zeroflag.Components.Logging.Log _Log;

		/// <summary>
		/// The log to write to.
		/// </summary>
		public zeroflag.Components.Logging.Log Log
		{
			get { return _Log; }
			set
			{
				if ( _Log != value )
				{
					this.OnLogChanged( _Log, _Log = value );
				}
			}
		}

		#region LogChanged event
		public delegate void LogChangedHandler( object sender, zeroflag.Components.Logging.Log oldvalue, zeroflag.Components.Logging.Log newvalue );

		private event LogChangedHandler _LogChanged;
		/// <summary>
		/// Occurs when Log changes.
		/// </summary>
		public event LogChangedHandler LogChanged
		{
			add { this._LogChanged += value; }
			remove { this._LogChanged -= value; }
		}

		/// <summary>
		/// Raises the LogChanged event.
		/// </summary>
		protected virtual void OnLogChanged( zeroflag.Components.Logging.Log oldvalue, zeroflag.Components.Logging.Log newvalue )
		{
			if ( oldvalue != null )
				this.OnLogUnregister( oldvalue );
			if ( newvalue != null )
				this.OnLogRegister( newvalue );

			// if there are event subscribers...
			if ( this._LogChanged != null )
			{
				// call them...
				this._LogChanged( this, oldvalue, newvalue );
			}
		}

		protected virtual void OnLogUnregister( zeroflag.Components.Logging.Log value )
		{
		}

		protected virtual void OnLogRegister( zeroflag.Components.Logging.Log value )
		{
		}

		#endregion LogChanged event
		#endregion Log

		#region LastWrite
		private Time _LastWrite;

		/// <summary>
		/// When the write occured.
		/// </summary>
		public Time LastWrite
		{
			get { return _LastWrite; }
			set
			{
				if ( _LastWrite != value )
				{
					_LastWrite = value;
				}
			}
		}

		#endregion LastWrite

		#region Interval
		private TimeSpan? _Interval;

		/// <summary>
		/// Interval between writes.
		/// </summary>
		public TimeSpan Interval
		{
			get { return _Interval ?? ( _Interval = this.IntervalCreate ) ?? TimeSpan.FromSeconds( 1 ); }
			set
			{
				if ( _Interval != value )
				{
					//if (_Interval != null) { }
					_Interval = value;
					//if (_Interval != null) { }
				}
			}
		}

		/// <summary>
		/// Creates the default/initial value for Interval.
		/// Interval between writes.
		/// </summary>
		protected virtual TimeSpan? IntervalCreate
		{
			get
			{
				TimeSpan? value = _Interval = TimeSpan.FromSeconds( 1 );
				return value;
			}
		}

		#endregion Interval

		#region Items
		private zeroflag.Collections.Dictionary<string, PerformanceLogItem> _Items;

		/// <summary>
		/// PerformanceLogItems by name.
		/// </summary>
		public zeroflag.Collections.Dictionary<string, PerformanceLogItem> Items
		{
			get { return _Items ?? ( _Items = this.ItemsCreate ); }
			//protected set
			//{
			//	if (_Items != value)
			//	{
			//		//if (_Items != null) { }
			//		_Items = value;
			//		//if (_Items != null) { }
			//	}
			//}
		}

		/// <summary>
		/// Creates the default/initial value for Items.
		/// PerformanceLogItems by name.
		/// </summary>
		protected virtual zeroflag.Collections.Dictionary<string, PerformanceLogItem> ItemsCreate
		{
			get
			{
				zeroflag.Collections.Dictionary<string, PerformanceLogItem> value = _Items = new zeroflag.Collections.Dictionary<string, PerformanceLogItem>();
				value.DefaultValue = name => new PerformanceLogItem() { Name = name };
				return value;
			}
		}


		public PerformanceLogItem this[string name]
		{
			get { return this.Items[name]; }
		}

		#endregion Items

		protected override void OnUpdate( TimeSpan timeSinceLastUpdate )
		{
			var now = Time.Now;

			if ( now - this.LastWrite > this.Interval )
			{
				this.LastWrite = now;
				this.WriteLog();
			}

			base.OnUpdate( timeSinceLastUpdate );
		}

		private void WriteLog()
		{
			StringBuilder b = new StringBuilder();
			b.Append( "Performance:" ).AppendLine();

			foreach ( var item in this.Items.Values )
			{
				b.Append( "\t" );
				item.ToString( b );
				b.AppendLine();
			}

			this.Log.Message( b );
		}
	}
}
