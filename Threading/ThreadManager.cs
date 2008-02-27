using System;
using System.Collections.Generic;
using System.Threading;

namespace zeroflag.Threading
{
	public class ThreadManager
	{
		private IDList<Thread> _Threads = null;

		protected internal IDList<Thread> Threads
		{
			get { return _Threads ?? CreateThreads(DefaultThreadCount); }
		}

		IDList<System.Threading.Thread> _NativeThreads = new IDList<System.Threading.Thread>();

		public IDList<System.Threading.Thread> NativeThreads
		{
			get { return _NativeThreads; }
		}

		protected virtual IDList<Thread> CreateThreads(int count)
		{
			if (this._Threads == null || this._Threads.Count != count)
				lock (this)
					if (this._Threads == null || this._Threads.Count != count)
					{
						Thread[] threads = new Thread[count];
						for (int i = 0; i < count; i++)
						{
							threads[i] = new Thread(this);
							if (!this.NativeThreads.Contains(threads[i].Native))
								this.NativeThreads.Add(threads[i].Native);
						}
						this._Threads = new IDList<Thread>(threads);
					}
			return this._Threads;
		}

		public ThreadManager(int numberOfThreads)
			: this()
		{
			this.CreateThreads(numberOfThreads);
		}

		public ThreadManager()
		{
			Current = this;
		}

		protected internal int GetInternalId(Thread thread)
		{
			return this.Threads.IndexOf(thread);
		}

		int _ScheduleLockCount = 0;
		protected internal void Schedule()
		{
			if (Interlocked.Increment(ref _ScheduleLockCount) < 2)
			{
				//TODO: do scheduling...
				throw new System.NotImplementedException();
			}
			Interlocked.Decrement(ref _ScheduleLockCount);
		}

		#region Current

		public readonly int DefaultThreadCount = System.Environment.ProcessorCount * 2;

		private static ThreadManager _Current = null;
		/// <summary>
		/// The ThreadManager instance used in the current context(appdomain).
		/// </summary>
		public static ThreadManager Current
		{
			get
			{
				if (_Current == null)
				{
					lock (typeof(ThreadManager))
					{
						_Current = new ThreadManager();
					}
				}
				return _Current;
			}
			protected set
			{
				_Current = value;
			}
		}
		#endregion Current

	}
}
