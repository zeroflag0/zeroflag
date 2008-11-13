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
				_Cancel = value;
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
			this.Tasks.Add( task );
		}

		public void Add( params Action[] tasks )
		{
			this.Tasks.AddRange( tasks );
		}

		public void Add<T1>( Action<T1> task, T1 p1 )
		{
			this.Tasks.Add( () => task( p1 ) );
		}

		public void Add<T1, T2>( Action<T1, T2> task, T1 p1, T2 p2 )
		{
			this.Tasks.Add( () => task( p1, p2 ) );
		}

		public void Add<T1, T2, T3>( Action<T1, T2, T3> task, T1 p1, T2 p2, T3 p3 )
		{
			this.Tasks.Add( () => task( p1, p2, p3 ) );
		}

		public void Add<T1, T2, T3, T4>( Action<T1, T2, T3, T4> task, T1 p1, T2 p2, T3 p3, T4 p4 )
		{
			this.Tasks.Add( () => task( p1, p2, p3, p4 ) );
		}

		private zeroflag.Collections.List<Action> _Tasks;

		/// <summary>
		/// Tasks to be processed in the background.
		/// </summary>
		public zeroflag.Collections.List<Action> Tasks
		{
			get { return _Tasks ?? ( _Tasks = this.TasksCreate ); }
		}

		/// <summary>
		/// Creates the default/initial value for Tasks.
		/// Tasks to be processed in the background.
		/// </summary>
		protected virtual zeroflag.Collections.List<Action> TasksCreate
		{
			get
			{
				var list = new zeroflag.Collections.List<Action>() { };
				list.ItemAdded += item =>
				{
					if ( !this.backgroundWorker.IsBusy || _Working < 1 )
					{
						try
						{
							this.backgroundWorker.RunWorkerAsync();
						}
#if DEBUG || TRACE || VERBOSE
						catch ( Exception exc )
						{
							Console.WriteLine(exc);
						}
#else
						catch {}
#endif
					}
					_wait.Set();
				};
				return list;
			}
		}

		#endregion Tasks

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


		System.Threading.AutoResetEvent _wait = new System.Threading.AutoResetEvent( false );
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
				while ( !this.Cancel || this.Tasks.Count > 0 )
				{
					if ( this.Tasks.Count > 0 )
					{
						_LastWork = DateTime.Now;
						current = this.Tasks[0];
						if ( current != null )
						{
							try
							{
								current();
							}
							catch ( Exception exc )
							{
								this.OnErrorHandling( current, exc );
							}
						}
						this.Tasks.RemoveAt( 0 );
					}
					else
					{
						if ( DateTime.Now - _LastWork > this.IdleThreadTimeout && !this.Cancel )
						{
							Console.WriteLine( "TaskProcessor(" + this.Name + ") idle timeout." );
							this._wait.WaitOne( 180000, true );
							Console.WriteLine( "TaskProcessor(" + this.Name + ") resuming..." );
							//return;
						}
						else
							this._wait.WaitOne( 200, true );
					}
				}
				Console.WriteLine( "TaskProcessor(" + this.Name + ") halted." );
			}
			finally
			{
				System.Threading.Interlocked.Decrement( ref _Working );
			}
		}

		protected virtual void OnDispose()
		{
			this.Cancel = true;
			while ( this.backgroundWorker.IsBusy && _Working > 0 )
			{
				_wait.Set();
				System.Threading.Thread.Sleep( 50 );
			}
		}


	}
}
