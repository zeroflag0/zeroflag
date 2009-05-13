using System;

namespace System.Diagnostics
{
#if SILVERLIGHT
	public class Stopwatch
	{
		/// <summary>
		/// Indicates whether the timer is based on a high-resolution performance counter. This field is read-only. 
		/// </summary>
		public static readonly bool IsHighResolution = false;

		/// <summary>
		/// Gets the frequency of the timer as the number of ticks per second. This field is read-only. 
		/// </summary>
		public static readonly long Frequency = 50;


		#region StartTime
		private Time _StartTime;

		/// <summary>
		/// StartTime
		/// </summary>
		public Time StartTime
		{
			get { return _StartTime; }
			protected set
			{
				if ( _StartTime != value )
				{
					_StartTime = value;
				}
			}
		}

		#endregion StartTime

		#region IsRunning
		private bool _IsRunning;

		/// <summary>
		/// Gets a value indicating whether the Stopwatch timer is running.
		/// </summary>
		public bool IsRunning
		{
			get { return _IsRunning; }
			set
			{
				if ( _IsRunning != value )
				{
					_IsRunning = value;
				}
			}
		}

		#endregion IsRunning

		#region Elapsed
		Time _Elapsed;
		/// <summary>
		/// Gets the total elapsed time measured by the current instance. 
		/// </summary>
		public Time Elapsed
		{
			get
			{
				if ( this.IsRunning )
					_Elapsed = Time.Now - this.StartTime;
				return _Elapsed;
			}
			protected set { _Elapsed = value; }
		}

		#endregion Elapsed

		#region ElapsedMilliseconds
		/// <summary>
		/// Gets the total elapsed time measured by the current instance, in milliseconds.
		/// </summary>
		public long ElapsedMilliseconds
		{
			get { return this.Elapsed; }
		}

		#endregion ElapsedMilliseconds

		/// <summary>
		/// Starts, or resumes, measuring elapsed time for an interval. 
		/// </summary>
		public void Start()
		{
			this.IsRunning = true;
			this.StartTime = Time.Now;
		}

		/// <summary>
		/// Stops measuring elapsed time for an interval. 
		/// </summary>
		public void Stop()
		{
			this.IsRunning = false;
			this.Elapsed.GetType();
		}

		/// <summary>
		/// Stops time interval measurement and resets the elapsed time to zero. 
		/// </summary>
		public void Reset()
		{
			this.StartTime = _Elapsed = Time.Now;
			this.IsRunning = false;
		}
	}
#endif
}
