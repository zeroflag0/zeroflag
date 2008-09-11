using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Threading
{
	public class Thread
	{
		private ThreadManager _Manager;
		private System.Threading.AutoResetEvent _WaitHandler = new System.Threading.AutoResetEvent( false );
		private Task _CurrentTask;
		private Task _NextTask;
		private System.Threading.Thread _Native;

		public System.Threading.Thread Native
		{
			get { return _Native; }
		}

		protected internal Task NextTask
		{
			get { return _NextTask; }
			set { _NextTask = value; }
		}

		protected internal Task CurrentTask
		{
			get { return _CurrentTask; }
			set { _CurrentTask = value; }
		}

		protected internal System.Threading.AutoResetEvent WaitHandler
		{
			get { return _WaitHandler; }
		}

		public ThreadManager Manager
		{
			get { return _Manager; }
		}

		public int InternalId
		{
			get
			{
				return this.Manager.GetInternalId( this );
			}
		}

		public static int CurrentInternalId
		{
			get
			{
				System.Threading.Thread current = System.Threading.Thread.CurrentThread;
				Thread result = ThreadManager.Current.Threads.Find( delegate( Thread t ) { return t != null && t.Native == current; } );
				if ( result == null )
					return -1;
				else
					return ThreadManager.Current.Threads[ result ];
			}
		}

		public int CurrentManagId
		{
			get
			{
				return System.Threading.Thread.CurrentThread.ManagedThreadId;
			}
		}

		public int ManagedId
		{
			get
			{
				return this.Native.ManagedThreadId;
			}
		}

		public bool IsBusy
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		public bool IsWaiting
		{
			get
			{
				throw new System.NotImplementedException();
			}
			set
			{
			}
		}

		protected virtual void Run()
		{
			this.WaitHandler.WaitOne();
		}

		protected internal Thread( ThreadManager manager )
		{
			this._Manager = manager;
			this._Native = new System.Threading.Thread( this.Run );
		}
	}
}
