﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zeroflag.Components
{
	public struct Task
	{
		#region Action
		private Action _Action;

		/// <summary>
		/// The task's action delegate.
		/// </summary>
		public Action Action
		{
			get { return _Action; }
			set
			{
				if ( _Action != value )
				{
					_Action = value;
				}
			}
		}

		#endregion Action

		#region Info
		private System.Reflection.MethodInfo _Info;

		/// <summary>
		/// Information about the task's (unwrapped) action delegate.
		/// </summary>
		public System.Reflection.MethodInfo Info
		{
			get { return _Info; }
			set
			{
				if ( _Info != value )
				{
					_Info = value;
				}
			}
		}

		#endregion Info

		#region Time
		private DateTime _Time;

		/// <summary>
		/// When the task is scheduled for execution.
		/// </summary>
		public DateTime Time
		{
			get { return _Time; }
			set
			{
				if ( _Time != value )
				{
					_Time = value;
				}
			}
		}

		#endregion Time


		public override string ToString()
		{
			return ( this.Time != DateTime.MinValue ? this.Time.ToString( "HH:mm:ss." ) + this.Time.Millisecond : "" ) + "[" + this.Info.DeclaringType + "." + this.Info.ToString() + ( this.Info != this.Action.Method ? " => " + this.Action.Method.ToString() : "" );
		}

		public Task( DateTime time, Action action, System.Reflection.MethodInfo info )
		{
			_Action = action;
			_Info = info;
			_Time = time;
		}

		public Task( Action action, System.Reflection.MethodInfo info )
			: this( DateTime.MinValue, action, info )
		{
		}

		public Task( Action action )
			: this( action, action.Method )
		{
		}
		public Task( DateTime time, Action action )
			: this( time, action, action.Method )
		{
		}

		public static Task Create( Action action )
		{
			return new Task( action, action.Method );
		}
		public static Task Create<T1>( Action<T1> action, T1 p1 )
		{
			return new Task( () => action( p1 ), action.Method );
		}
		public static Task Create<T1, T2>( Action<T1, T2> action, T1 p1, T2 p2 )
		{
			return new Task( () => action( p1, p2 ), action.Method );
		}
		public static Task Create<T1, T2, T3>( Action<T1, T2, T3> action, T1 p1, T2 p2, T3 p3 )
		{
			return new Task( () => action( p1, p2, p3 ), action.Method );
		}
		public static Task Create<T1, T2, T3, T4>( Action<T1, T2, T3, T4> action, T1 p1, T2 p2, T3 p3, T4 p4 )
		{
			return new Task( () => action( p1, p2, p3, p4 ), action.Method );
		}

		public static Task Create( DateTime time, Action action )
		{
			return new Task( time, action, action.Method );
		}
		public static Task Create<T1>( DateTime time, Action<T1> action, T1 p1 )
		{
			return new Task( time, () => action( p1 ), action.Method );
		}
		public static Task Create<T1, T2>( DateTime time, Action<T1, T2> action, T1 p1, T2 p2 )
		{
			return new Task( time, () => action( p1, p2 ), action.Method );
		}
		public static Task Create<T1, T2, T3>( DateTime time, Action<T1, T2, T3> action, T1 p1, T2 p2, T3 p3 )
		{
			return new Task( time, () => action( p1, p2, p3 ), action.Method );
		}
		public static Task Create<T1, T2, T3, T4>( DateTime time, Action<T1, T2, T3, T4> action, T1 p1, T2 p2, T3 p3, T4 p4 )
		{
			return new Task( time, () => action( p1, p2, p3, p4 ), action.Method );
		}

		public static implicit operator Task( Action action )
		{
			return new Task( action );
		}
	}
}
