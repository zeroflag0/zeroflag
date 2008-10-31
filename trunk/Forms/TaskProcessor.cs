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
				if ( value )
				{
					this.Tasks.Clear();
					this.backgroundWorker.CancelAsync();
				}
				else
					this.backgroundWorker.RunWorkerAsync();
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
					if ( !this.backgroundWorker.IsBusy )
						this.backgroundWorker.RunWorkerAsync();
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
		DateTime _LastWork;
		void backgroundWorker_DoWork( object sender, DoWorkEventArgs e )
		{
			Console.WriteLine( "TaskProcessor(" + this.Name + ") started..." );
			while ( /*this.Tasks.Count > 0 && */!this.Cancel )
			{
				if ( this.Tasks.Count > 0 )
				{
					_LastWork = DateTime.Now;
					if ( this.Tasks[0] != null )
						this.Tasks[0]();
					this.Tasks.RemoveAt( 0 );
				}
				else
				{
					if ( DateTime.Now - _LastWork > this.IdleThreadTimeout )
					{
						Console.WriteLine( "TaskProcessor(" + this.Name + ") idle timeout." );
						return;
					}
					this._wait.WaitOne( 200, true );
				}
			}
			Console.WriteLine( "TaskProcessor(" + this.Name + ") halted." );
		}

	}
}
