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
	static Time()
	{
		_TimeOffset = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
		_TimeStopwatch.Start();

		//TestTime();
	}

	public static void TestTime()
	{
		DateTime dtstart = DateTime.Now;
		var ticks = dtstart.Ticks;
		Time start = Time.Now;

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
		var d = dtend- dtstart;
		if (dtstart.Ticks + d.Ticks != dtend.Ticks)
			throw new PlatformNotSupportedException( "TimeSpan.TicksPerMillisecond does not apply." );

		Console.WriteLine( "Timing OK" );
	}

	static System.Diagnostics.Stopwatch _TimeStopwatch = new System.Diagnostics.Stopwatch();
	static long _TimeOffset;

	public static long NowMs
	{
		get { return _TimeOffset + _TimeStopwatch.ElapsedMilliseconds; }
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
	#endregion Now

	public override string ToString()
	{
		if ( this._Value < 500 )
			return _Value.ToString( "##0" ) + "ms";

		DateTime t = (DateTime)this;
		if ( t.Date != DateTime.Today )
			return t.ToString( "yyyy-MM-dd HH:mm:ss." ) + t.Millisecond.ToString( "000" ) + "h";
		else if ( t.Hour > 0 )
			return t.ToString( "HH:mm:ss." ) + t.Millisecond.ToString( "000" ) + "h";
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
