using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace zeroflag.Forms
{
	public partial class TaskProcessor : Component
	{
		public TaskProcessor()
		{
			InitializeComponent();
		}

		public TaskProcessor( IContainer container )
		{
			container.Add( this );

			InitializeComponent();
		}

		bool _Cancel = false;
		public bool Cancel
		{
			get { return _Cancel || this.backgroundWorker.CancellationPending; }
			set
			{
				if ( value != _Cancel )
				{
#if DEBUG && false
					Console.WriteLine( ( this.Name ?? this.ToString() ) + ".Cancel = " + value );
					Console.WriteLine( new System.Diagnostics.StackTrace( 1 ) );
#endif
					_Cancel = value;
					if ( _Cancel )
						this.CancelRequestTime = DateTime.Now;
					else
						this.CancelRequestTime = null;
				}
				//Console.WriteLine( this + ".Cancel = " + value );
				//if ( value )
				//{
				//    this.Tasks.Clear();
				//    this.backgroundWorker.CancelAsync();
				//}
				//else
				//this.backgroundWorker.RunWorkerAsync();
			}
		}

		public bool IsRunning
		{
			get { return this.backgroundWorker.IsBusy; }
		}

		#region Name

		private string _Name;

		/// <summary>
		/// This taskprocessor's name (optional).
		/// </summary>
		public string Name
		{
			get { return _Name; }
			set
			{
				if ( _Name != value )
				{
					_Name = value;
					if ( _Tasks != null )
						this.Add( () => System.Threading.Thread.CurrentThread.Name = value );
				}
			}
		}

		#endregion Name

		#region event ErrorHandling
		public delegate void ErrorHandlingHandler( Action task, Exception exc );

		private event ErrorHandlingHandler _ErrorHandling;
		/// <summary>
		/// When an error occurs during task execution.
		/// </summary>
		public event ErrorHandlingHandler ErrorHandling
		{
			add { this._ErrorHandling += value; }
			remove { this._ErrorHandling -= value; }
		}
		/// <summary>
		/// Call to raise the ErrorHandling event:
		/// When an error occurs during task execution.
		/// </summary>
		protected virtual void OnErrorHandling( Action task, Exception exc )
		{
			// if there are event subscribers...
			if ( this._ErrorHandling != null )
			{
				// call them...
				this._ErrorHandling( task, exc );
			}
		}
		#endregion event ErrorHandling

		#region Tasks
		public void Add( Action task )
		{
			this.Tasks.Write( task );
			if ( !this.backgroundWorker.IsBusy || _Working < 1 )
			{
				try
				{
					this.backgroundWorker.RunWorkerAsync();
				}
#if DEBUG
							//|| TRACE || VERBOSE
						catch ( Exception exc )
						{
							Console.WriteLine( exc );
						}
#else
				catch { }
#endif
			}
			Wait.Set();
		}

		public void Add( params Action[] tasks )
		{
			foreach ( var task in tasks )
				this.Add( task );
			//this.Tasks.AddRange( tasks );
		}

		public void Add<T1>( Action<T1> task, T1 p1 )
		{
			this.Add( () => task( p1 ) );
		}

		public void Add<T1, T2>( Action<T1, T2> task, T1 p1, T2 p2 )
		{
			this.Add( () => task( p1, p2 ) );
		}

		public void Add<T1, T2, T3>( Action<T1, T2, T3> task, T1 p1, T2 p2, T3 p3 )
		{
			this.Add( () => task( p1, p2, p3 ) );
		}

		public void Add<T1, T2, T3, T4>( Action<T1, T2, T3, T4> task, T1 p1, T2 p2, T3 p3, T4 p4 )
		{
			this.Add( () => task( p1, p2, p3, p4 ) );
		}

		public int Count
		{
			get { return this.Tasks.Count; }
		}

		public void Clear()
		{
			this.Tasks.Clear();
		}

		private zeroflag.Threading.LocklessQueue<Action> _Tasks;

		/// <summary>
		/// Tasks to be processed in the background.
		/// </summary>
		protected zeroflag.Threading.LocklessQueue<Action> Tasks
		{
			get { return _Tasks ?? ( _Tasks = this.TasksCreate ); }
		}

		/// <summary>
		/// Creates the default/initial value for Tasks.
		/// Tasks to be processed in the background.
		/// </summary>
		protected virtual zeroflag.Threading.LocklessQueue<Action> TasksCreate
		{
			get
			{
				var list = new zeroflag.Threading.LocklessQueue<Action>() { };
#if OBSOLETE
				list.ItemAdded += item =>
				{
					if ( !this.backgroundWorker.IsBusy || _Working < 1 )
					{
						try
						{
							this.backgroundWorker.RunWorkerAsync();
						}
#if DEBUG
							//|| TRACE || VERBOSE
						catch ( Exception exc )
						{
							Console.WriteLine( exc );
						}
#else
						catch {}
#endif
					}
					Wait.Set();
				};
#endif
				if ( _Name != null )
					list.Write( () => System.Threading.Thread.CurrentThread.Name = this.Name );
				return list;
			}
		}

		#endregion Tasks

		#region CancelTimeout
		#region CancelRequestTime

		private DateTime? _CancelRequestTime;

		/// <summary>
		/// When was the Cancel issued?
		/// </summary>
		public DateTime? CancelRequestTime
		{
			get { return _CancelRequestTime; }
			set
			{
				if ( _CancelRequestTime != value )
				{
					_CancelRequestTime = value;
				}
			}
		}

		#endregion CancelRequestTime

		private TimeSpan _CancelTimeout = TimeSpan.FromSeconds( 10 );

		/// <summary>
		/// Time for how long the thread will be allowed to contineue after Cancel has been set.
		/// </summary>
		public TimeSpan CancelTimeout
		{
			get { return _CancelTimeout; }
			set
			{
				if ( _CancelTimeout != value )
				{
					_CancelTimeout = value;
				}
			}
		}

		#endregion CancelTimeout


		#region IdleThreadTimeout

		private TimeSpan _IdleThreadTimeout = TimeSpan.FromSeconds( 2.0 );

		/// <summary>
		/// Time after which the worker thread gets suspended (leaves active waiting).
		/// </summary>
		public TimeSpan IdleThreadTimeout
		{
			get { return _IdleThreadTimeout; }
			set
			{
				if ( _IdleThreadTimeout != value )
				{
					_IdleThreadTimeout = value;
				}
			}
		}

		#endregion IdleThreadTimeout

#if DEBUG
		#region DebugTrace

		private static bool _DebugTrace;

		/// <summary>
		/// Trace the current state of the worker.
		/// </summary>
		public static bool DebugTrace
		{
			get { return _DebugTrace; }
			set
			{
				if ( _DebugTrace != value )
				{
					_DebugTrace = value;
					if ( value )
					{
						lock ( _WaitHandles )
						{
							foreach ( var handle in _WaitHandles )
							{
								handle.Set();
							}
						}
					}
				}
			}
		}

		#endregion DebugTrace
		private static List<System.Threading.AutoResetEvent> _WaitHandles = new List<System.Threading.AutoResetEvent>();
#endif

		//System.Threading.AutoResetEvent Wait = new System.Threading.AutoResetEvent( false );

		#region Wait
		private System.Threading.AutoResetEvent _Wait;

		/// <summary>
		/// Wait handle.
		/// </summary>
		protected System.Threading.AutoResetEvent Wait
		{
			get { return _Wait ?? ( _Wait = this.WaitCreate ); }
			//set { _Wait = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Wait.
		/// Wait handle.
		/// </summary>
		protected virtual System.Threading.AutoResetEvent WaitCreate
		{
			get
			{
				var Wait = _Wait = new System.Threading.AutoResetEvent( false );
#if DEBUG
				lock ( _WaitHandles )
				{
					_WaitHandles.Add( _Wait );
				}
#endif
				return Wait;
			}
		}

		#endregion Wait


		DateTime _LastWork = DateTime.Now;
		int _Working = 0;
		void backgroundWorker_DoWork( object sender, DoWorkEventArgs e )
		{
			try
			{
				Action current;
				System.Threading.Interlocked.Increment( ref _Working );
				if ( _Working > 1 )
					return;
				Console.WriteLine( "TaskProcessor(" + this.Name + ") started..." );
				while ( !this.Cancel || !this.Tasks.IsEmpty )
				{
					if ( this.Cancel && ( DateTime.Now - this.CancelRequestTime ).Value.TotalMilliseconds > this.CancelTimeout.TotalMilliseconds * 0.85 )
					{
						Console.WriteLine( "TaskProcessor(" + this.Name + ") canceled..." );
						break;
					}
					if ( this.Tasks.Count > 0 )
					{
						//current = this.Tasks[0];
						current = this.Tasks.Read();
						if ( current != null )
						{
							try
							{
								current();
							}
							catch ( Exception exc )
							{
								Console.WriteLine( exc );
								this.OnErrorHandling( current, exc );
							}
						}
						_LastWork = DateTime.Now;
						//this.Tasks.RemoveAt( 0 );
					}
					else
					{
						if ( DateTime.Now - _LastWork > this.IdleThreadTimeout && !this.Cancel )
						{
#if DEBUG
							Console.WriteLine( "TaskProcessor(" + this.Name + ") idle timeout." );
#endif
							this.Wait.WaitOne( 15000, true );
#if DEBUG
							Console.WriteLine( "TaskProcessor(" + this.Name + ") resuming..." );
#endif
							_LastWork = DateTime.Now;
							//return;
						}
						else
							this.Wait.WaitOne( 200, true );
					}
#if DEBUG
					if ( DebugTrace )
						Console.WriteLine( new System.Diagnostics.StackTrace() );
#endif
				}
				Console.WriteLine( "TaskProcessor(" + this.Name + ") halted." );
			}
			finally
			{
				System.Threading.Interlocked.Decrement( ref _Working );
			}
		}

		public override string ToString()
		{
			return ( (object)this.Name ?? base.ToString() ).ToString();
		}
		protected virtual void OnDispose()
		{
			Console.WriteLine( this + ".OnDispose()" );
			this.Cancel = true;
			while ( this.backgroundWorker.IsBusy && _Working > 0 )
			{
				Wait.Set();
				System.Threading.Thread.Sleep( 50 );
				if ( DateTime.Now - this.CancelRequestTime > this.CancelTimeout )
				{
					Console.WriteLine( ( this.Name ?? this.ToString() ) + "CancelTimeout" );
					try
					{
						this.backgroundWorker.CancelAsync();
					}
					catch ( Exception exc )
					{
						Console.WriteLine( exc );
					}
					break;
				}
			}
		}

	}
}
