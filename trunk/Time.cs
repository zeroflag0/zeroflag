#if WIN32
#define WIN32_TIMING
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct Time : IComparable<Time>, IComparable<double>, IComparable<long>, IComparable<TimeSpan>, IComparable<DateTime>
{
	#region Value
	private long _Value;

	/// <summary>
	/// The time value in milliseconds.
	/// </summary>
	public long Value
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

	#region Total
	public double TotalMilliseconds
	{
		get { return _Value; }
	}
	#endregion Total

	#region Add
	public Time AddSeconds( double t )
	{
		return this.AddMilliseconds( t * 1000 );
	}
	public Time AddMilliseconds( double t )
	{
		return this.AddMilliseconds( (long)t );
	}
	public Time AddMilliseconds( long t )
	{
		return this._Value += t * TimeSpan.TicksPerMillisecond;
	}
	#endregion Add

	#region Operators
	#region +
	public static Time operator +( Time a, Time b )
	{
		return a._Value + b._Value;
	}
	//public static Time operator +( Time a, long b )
	//{
	//    return a._Value + b;
	//}
	//public static Time operator +( long a, Time b )
	//{
	//    return a + b._Value;
	//}
	public static Time operator +( Time a, TimeSpan b )
	{
		return a._Value + b.TotalMilliseconds;
	}
	//public static Time operator +( Time a, DateTime b )
	//{
	//    return a._Value + (Time)b;
	//}
	//public static Time operator +( DateTime a, Time b )
	//{
	//    return (Time)a + b._Value;
	//}
	#endregion +

	#region -
	//public static Time operator -( Time a, long b )
	//{
	//    return a._Value - b;
	//}
	//public static Time operator -( long a, Time b )
	//{
	//    return a - b._Value;
	//}
	public static Time operator -( Time a, TimeSpan b )
	{
		return a._Value - b.TotalMilliseconds;
	}
	public static Time operator -( TimeSpan a, Time b )
	{
		return a.TotalMilliseconds - b._Value;
	}

	public static Time operator -( Time a, Time b )
	{
		return TimeSpan.FromMilliseconds( a._Value - b._Value );
	}
	//public static Time operator -( Time a, DateTime b )
	//{
	//    return a._Value - (Time)b;
	//}
	//public static Time operator -( DateTime a, Time b )
	//{
	//    return (Time)a - b._Value;
	//}
	#endregion -

	#region *
	public static Time operator *( Time a, Time b )
	{
		return a._Value * b._Value;
	}
	//public static Time operator *( Time a, long b )
	//{
	//    return a._Value * b;
	//}
	//public static Time operator *( long a, Time b )
	//{
	//    return a * b._Value;
	//}
	//public static Time operator *( Time a, double b )
	//{
	//    return a._Value * b;
	//}
	//public static Time operator *( double a, Time b )
	//{
	//    return a * b._Value;
	//}
	#endregion *

	#region /
	public static Time operator /( Time a, Time b )
	{
		return a._Value / b._Value;
	}
	//public static Time operator /( Time a, long b )
	//{
	//    return a._Value / b;
	//}
	//public static Time operator /( long a, Time b )
	//{
	//    return a / b._Value;
	//}
	//public static Time operator /( Time a, double b )
	//{
	//    return a._Value / b;
	//}
	//public static Time operator /( double a, Time b )
	//{
	//    return a / b._Value;
	//}
	#endregion /
	#endregion Operators

	#region Comparisons
	public static bool operator ==( Time a, Time b )
	{
		return a._Value == b._Value;
	}
	public static bool operator !=( Time a, Time b )
	{
		return a._Value != b._Value;
	}
	public static bool operator <( Time a, Time b )
	{
		return a._Value < b._Value;
	}
	public static bool operator >( Time a, Time b )
	{
		return a._Value > b._Value;
	}
	public static bool operator <=( Time a, Time b )
	{
		return a._Value <= b._Value;
	}
	public static bool operator >=( Time a, Time b )
	{
		return a._Value >= b._Value;
	}
	#endregion Comparisons

	#region Implicit Conversion
	public static implicit operator Time( long time )
	{
		return new Time() { _Value = time };
	}
	public static implicit operator long( Time time )
	{
		return time._Value;
	}
	public static implicit operator Time( double time )
	{
		return (long)time;
	}
	public static implicit operator double( Time time )
	{
		return time._Value;
	}
	public static implicit operator Time( int time )
	{
		return (long)time;
	}
	public static implicit operator int( Time time )
	{
		return (int)time._Value;
	}
	public static implicit operator Time( float time )
	{
		return (long)time;
	}
	public static implicit operator float( Time time )
	{
		return (float)time._Value;
	}
	public static explicit operator DateTime( Time time )
	{
		return new DateTime( time._Value * TimeSpan.TicksPerMillisecond );
	}
	public static implicit operator Time( DateTime time )
	{
		return time.Ticks / TimeSpan.TicksPerMillisecond;
	}
	public static explicit operator TimeSpan( Time time )
	{
		return TimeSpan.FromMilliseconds( time._Value );
	}
	public static implicit operator Time( TimeSpan time )
	{
		return time.TotalMilliseconds;
	}
	#endregion Implicit Conversion

	#region Now
#if WIN32_TIMING
	[System.Runtime.InteropServices.DllImport( "winmm.dll", EntryPoint = "timeBeginPeriod" )]
	static extern uint MM_BeginPeriod( uint uMilliseconds );


	[System.Runtime.InteropServices.DllImport( "winmm.dll", EntryPoint = "timeEndPeriod" )]
	static extern uint MM_EndPeriod( uint uMilliseconds );


	[System.Runtime.InteropServices.DllImport( "winmm.dll", EntryPoint = "timeGetTime" )]
	static extern uint MM_GetTime();
#endif

	static Time()
	{
		Time.Now.GetType();
#if !SILVERLIGHT && !NO_HIGH_PRECISION_TIMING && !WIN32_TIMING
		//_TimeOffset = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		//TimeStopwatch.Start();
#elif WIN32_TIMING
		MM_BeginPeriod( 1 );
		_DateTimeOffset = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		_TimeOffset = MM_GetTime();
#endif
		//TestTime();
	}

	public static void TestTime()
	{
		Console.WriteLine( ( IntPtr.Size * 8 ) + "bit" );
		DateTime dtstart = DateTime.Now;
		var ticks = dtstart.Ticks;
		Time start = Time.Now;
		long duration = 1000;
		Time end = start + duration;
		long ms = 0, msstart = start;
		while ( DateTime.Now < end )
		{
			ms += Time.Now - msstart;
			msstart = Time.Now;
			System.Threading.Thread.Sleep( 1 );
		}
		if ( Math.Abs( ms - duration ) > (double)duration * 0.05 )
			throw new PlatformNotSupportedException( System.Threading.Thread.CurrentThread.ManagedThreadId + " DateTime's timing differs from Stopwatch. (" + ms + " != " + duration + ", " + Time.Now + ")" );
		else
			Console.WriteLine( start + " .. " + end + "; " + ms + " ~= " + duration + " => Precision OK!" );

		return;
		if ( DateTime.Now - Time.Now > 10 )
			throw new PlatformNotSupportedException( "DateTime's timing differs from Stopwatch." );
		System.Threading.Thread.Sleep( 500 );

		if ( DateTime.Now - Time.Now > 10 )
			throw new PlatformNotSupportedException( "DateTime's timing differs from Stopwatch." );
		System.Threading.Thread.Sleep( 500 );

		if ( DateTime.Now - Time.Now > 10 )
			throw new PlatformNotSupportedException( "DateTime's timing differs from Stopwatch." );
		if ( Time.Now - 1000 - dtstart > 20 )
			throw new PlatformNotSupportedException( "DateTime's timing differs from Stopwatch. (Precision insufficient)" );
		if ( DateTime.Now - start - 1000.0 > 20 )
			throw new PlatformNotSupportedException( "DateTime's timing differs from Stopwatch. (Precision insufficient)" );
		if ( dtstart - start > 10 )
			throw new PlatformNotSupportedException( "DateTime's timing differs from Stopwatch." );

		var dtend = DateTime.Now;
		var d = dtend - dtstart;
		if ( dtstart.Ticks + d.Ticks != dtend.Ticks )
			throw new PlatformNotSupportedException( "TimeSpan.TicksPerMillisecond does not apply." );

		Console.WriteLine( "Timing OK" );
	}

#if !SILVERLIGHT && !NO_HIGH_PRECISION_TIMING && !WIN32_TIMING
	#region TimeStopwatch
	[System.ThreadStatic]
	private static System.Diagnostics.Stopwatch _TimeStopwatch;

	/// <summary>
	/// Stopwatch for high-resolution timing
	/// </summary>
	public static System.Diagnostics.Stopwatch TimeStopwatch
	{
		get { return _TimeStopwatch ?? ( _TimeStopwatch = TimeStopwatchInitialize( TimeStopwatchCreate ) ); }
		//protected 
		//set
		//{
		//	if (_TimeStopwatch != value)
		//	{
		//		//if (_TimeStopwatch != null) { }
		//		_TimeStopwatch = value;
		//		//if (_TimeStopwatch != null) { }
		//	}
		//}
	}

	/// <summary>
	/// Creates the default/initial value for TimeStopwatch.
	/// Stopwatch for high-resolution timing
	/// </summary>
	static System.Diagnostics.Stopwatch TimeStopwatchCreate
	{
		get
		{
			return new System.Diagnostics.Stopwatch();
		}
	}

	/// <summary>
	/// Initializes the default value for TimeStopwatch.
	/// Stopwatch for high-resolution timing
	/// </summary>
	static System.Diagnostics.Stopwatch TimeStopwatchInitialize( System.Diagnostics.Stopwatch v )
	{
		_TimeOffset = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		//Console.WriteLine( "Time initializing on thread " + System.Threading.Thread.CurrentThread.ManagedThreadId + " => " + _TimeOffset );
		v.Start();

		return v;
	}

	#endregion TimeStopwatch

	[System.ThreadStatic]
	static long _TimeOffset;

	public static long NowMs
	{
		get { return _TimeOffset + TimeStopwatch.ElapsedMilliseconds; }
	}

	public static Time Now
	{
		//get { return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond; }
		get { return NowMs; }
	}

	public static DateTime NowDateTime
	{
		get { return (DateTime)Now; }
	}
#elif WIN32_TIMING
	static long _DateTimeOffset;
	static long _TimeOffset;
	public static long NowMs
	{
		get { return _DateTimeOffset + ( MM_GetTime() - _TimeOffset ); }
	}

	public static Time Now
	{
		get { return NowMs; }
	}

	public static DateTime NowDateTime
	{
		get { return (DateTime)Now; }
	}
#else
	public static long NowMs
	{
		get { return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond; }
	}

	public static Time Now
	{
		//get { return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond; }
		get { return NowMs; }
	}

	public static DateTime NowDateTime
	{
		get { return DateTime.Now; }
	}
#endif

	#endregion Now

	public override string ToString()
	{
		if ( this._Value < 500 )
			return _Value.ToString( "##0" ) + "ms";

		DateTime t = (DateTime)this;
		if ( t.Date != DateTime.Today && t.Date != DateTime.MinValue.Date )
			return t.ToString( "yyyy-MM-dd HH:mm:ss." ) + t.Millisecond.ToString( "000" ) + "";
		else if ( t.Hour > 0 )
			return t.ToString( "HH:mm:ss." ) + t.Millisecond.ToString( "000" ) + "";
		else if ( t.Minute > 0 )
			return t.ToString( "mm:ss." ) + t.Millisecond.ToString( "000" ) + "min";
		else
			return t.ToString( "s." ) + t.Millisecond.ToString( "000" ) + "s";

		//return ( t.Hour > 0 ? t.ToString( "HH:mm:ss" ) : ( t.Minute > 0 ? t.ToString( "mm:ss" ) : t.ToString( "s." ) ) ) + t.Millisecond.ToString( "000" ) + "s";
	}
	public static implicit operator string( Time time )
	{
		return time.ToString();
	}


	#region IComparable<Time> Members

	public int CompareTo( Time other )
	{
		return this._Value.CompareTo( other._Value );
	}

	#endregion

	#region IComparable<double> Members

	public int CompareTo( double other )
	{
		return this.CompareTo( (Time)other );
	}

	#endregion

	#region IComparable<long> Members

	public int CompareTo( long other )
	{
		return this.CompareTo( (Time)other );
	}

	#endregion

	#region IComparable<TimeSpan> Members

	public int CompareTo( TimeSpan other )
	{
		return this.CompareTo( (Time)other );
	}

	#endregion

	#region IComparable<DateTime> Members

	public int CompareTo( DateTime other )
	{
		return this.CompareTo( (Time)other );
	}

	#endregion
}
