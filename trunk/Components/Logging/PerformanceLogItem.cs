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
	public class PerformanceLogItem
	{
		#region Name
		private string _Name;

		/// <summary>
		/// The item's name.
		/// </summary>
		public string Name
		{
			get { return _Name; }
			set
			{
				if ( _Name != value )
				{
					_Name = value;
				}
			}
		}

		#endregion Name

		#region Time
		private double? _Time;

		/// <summary>
		/// The (average) time recorded for this item.
		/// </summary>
		public double Time
		{
			get { return _Time ?? 1; }
			set
			{
				if ( _Time != value )
				{
					_Time = value;
				}
			}
		}

		protected void RecordTime( Time time )
		{
			var now = global::Time.Now;
			//time /= 2;
			this.Time = time;// ( ( _Time ?? time ) * 1.0 + time ) / 2.0;

			if ( _LastPerSecond2 == now )
				this.PerSecond = 1000;
			else if ( _LastPerSecond2 > 0 )
			{
				double ps = 2000.0 / ( now - _LastPerSecond2 );
				this.PerSecond = ps;//( ( _PerSecond ?? ps ) * 1.0 + ps ) / 2.0;
				//_PerSecondCount++;
				//if ( global::Time.Now - _LastPerSecond.Value > 100 )
				//{
				//    this.PerSecond = 100.0 / (double)_PerSecondCount;
				//    _PerSecondCount = 0;
				//    _LastPerSecond = global::Time.Now;
				//}
			}
			//else
			//{
			//_PerSecondCount = 0;
			_LastPerSecond2 = _LastPerSecond;
			_LastPerSecond = now;
			//}
		}

		#endregion Time

		#region PerSecond
		//int _PerSecondCount;
		Time _LastPerSecond;
		Time _LastPerSecond2;
		private double? _PerSecond;

		/// <summary>
		/// How many times per second (on average) a time has been recorded.
		/// </summary>
		public double PerSecond
		{
			get { return _PerSecond ?? 0.1; }
			set
			{
				if ( _PerSecond != value )
				{
					_PerSecond = value;
				}
			}
		}

		#endregion PerSecond


		#region Details
		private object _Details;

		/// <summary>
		/// Any details on the item.
		/// </summary>
		public object Details
		{
			get { return _Details; }
			set
			{
				if ( _Details != value )
				{
					_Details = value;
				}
			}
		}

		#endregion Details



		#region Record
		TimeScope _Record;
		/// <summary>
		/// Record the time for a scope: using(logItem.Record) { ...testcode... }
		/// </summary>
		public TimeScope Record
		{
			get { return ( _Record ?? ( _Record = new TimeScope( this ) ) ).Start(); }
		}

		public class TimeScope : IDisposable
		{
			#region Stopwatch
#if LOCALTIME
			private System.Diagnostics.Stopwatch _Stopwatch;

			/// <summary>
			/// The stopwatch used for timing.
			/// </summary>
			public System.Diagnostics.Stopwatch Stopwatch
			{
				get { return _Stopwatch ?? ( _Stopwatch = this.StopwatchCreate ); }
				//protected set
				//{
				//	if (_Stopwatch != value)
				//	{
				//		//if (_Stopwatch != null) { }
				//		_Stopwatch = value;
				//		//if (_Stopwatch != null) { }
				//	}
				//}
			}

			/// <summary>
			/// Creates the default/initial value for Stopwatch.
			/// The stopwatch used for timing.
			/// </summary>
			protected virtual System.Diagnostics.Stopwatch StopwatchCreate
			{
				get
				{
					System.Diagnostics.Stopwatch value = _Stopwatch = new System.Diagnostics.Stopwatch();
					return value;
				}
			}
#endif
			#endregion Stopwatch

			PerformanceLogItem _LogItem;
			public TimeScope( PerformanceLogItem item )
			{
				_LogItem = item;
			}

			bool active;
#if !LOCALTIME
			Time start;
#endif
			public TimeScope Start()
			{
				if ( active )
					throw new InvalidOperationException( "Scope is already active." );
				active = true;
#if LOCALTIME
				this.Stopwatch.Reset();
				this.Stopwatch.Start();
#else
				start = global::Time.Now;
#endif
				return this;
			}


			public void Dispose()
			{
				active = false;
#if LOCALTIME
				double time = this.Stopwatch.ElapsedMilliseconds;
				this.Stopwatch.Stop();
				_LogItem.RecordTime( time );
#else
				var end = global::Time.Now;
				_LogItem.RecordTime( end - start );
#endif
			}

		}

		#endregion Record

		public override string ToString()
		{
			//return base.ToString();
			return this.ToString( new StringBuilder() ).ToString();
		}

		public virtual StringBuilder ToString( StringBuilder b )
		{
			b = b ?? new StringBuilder();

			b.Append( this.Name ).Append( ": " ).Append( ( (Time)this.Time ).ToString() ).Append( " " ).Append( this.PerSecond.ToString( "##0" ) ).Append( "/s" );

			if ( this.Details != null )
			{
				b.Append( " (" + this.Details + ")" );
			}

			return b;
		}
	}
}
