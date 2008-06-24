using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace zeroflag.Forms
{
	public partial class ParallelTaskProcessor : Component
	{
		public ParallelTaskProcessor()
		{
			InitializeComponent();
		}

		public ParallelTaskProcessor( IContainer container )
		{
			container.Add( this );

			InitializeComponent();
		}


		#region Cancel

		private bool _Cancel;

		public bool Cancel
		{
			get { return _Cancel; }
			set
			{
				if ( _Cancel != value )
				{
					_Cancel = value;
					if ( value )
					{
						foreach ( var worker in this.Workers )
							worker.CancelAsync();
						lock ( this.Tasks )
							this.Tasks.Clear();
					}
					else
						this.Run();
				}
			}
		}
		#endregion Cancel

		//public bool Cancel
		//{
		//    get
		//    {
		//        foreach ( var worker in this.Workers )
		//            if ( worker.CancellationPending )
		//                return true;
		//        return false;
		//    }
		//    set
		//    {
		//        if ( value )
		//        {
		//            foreach ( var worker in this.Workers )
		//                worker.CancelAsync();
		//            lock ( this.Tasks )
		//                this.Tasks.Clear();
		//        }
		//        else
		//            this.Run();
		//    }
		//}

		public bool IsRunning
		{
			get
			{
				foreach ( var worker in this.Workers )
					if ( worker.IsBusy )
						return true;
				return false;
			}
		}


		#region NumberOfWorkers

		private int _NumberOfWorkers = 4;

		public int NumberOfWorkers
		{
			get { return _NumberOfWorkers; }
			set
			{
				if ( _NumberOfWorkers != value )
				{
					_NumberOfWorkers = value;
					this._Workers = null;
				}
			}
		}
		#endregion NumberOfWorkers


		#region Workers
		private List<BackgroundWorker> _ExistingWorkers;
		private List<BackgroundWorker> _Workers;

		/// <summary>
		/// The background workers employed by this processor.
		/// </summary>
		public List<BackgroundWorker> Workers
		{
			get { return _Workers ?? ( _Workers = this.WorkersCreate ); }
		}

		/// <summary>
		/// Creates the default/initial value for Workers.
		/// The background workers employed by this processor.
		/// </summary>
		protected virtual List<BackgroundWorker> WorkersCreate
		{
			get
			{
				var workers = new List<BackgroundWorker>();
				if ( _ExistingWorkers != null )
				{
					foreach ( var worker in _ExistingWorkers )
					{
						if ( workers.Count < _NumberOfWorkers )
						{
							workers.Add( worker );
						}
						else
						{
							worker.CancelAsync();
							worker.Dispose();
						}
					}

				}
				while ( workers.Count < _NumberOfWorkers )
				{
					var worker = new BackgroundWorker();
					worker.DoWork += backgroundWorker_DoWork;
					this.components.Add( worker );
					workers.Add( worker );
				}
				this._ExistingWorkers = workers;

				return workers;
			}
		}

		protected void Run()
		{
			foreach ( var worker in this.Workers )
				worker.RunWorkerAsync();
		}
		#endregion Workers


		#region Tasks
		public void Add( Action task )
		{
			lock ( this.Tasks )
				this.Tasks.Add( task );
		}

		public void Add( params Action[] tasks )
		{
			lock ( this.Tasks )
				this.Tasks.Add( tasks );
		}

		public void Add<T1>( Action<T1> task, T1 p1 )
		{
			lock ( this.Tasks )
				this.Tasks.Add( () => task( p1 ) );
		}

		public void Add<T1, T2>( Action<T1, T2> task, T1 p1, T2 p2 )
		{
			lock ( this.Tasks )
				this.Tasks.Add( () => task( p1, p2 ) );
		}

		public void Add<T1, T2, T3>( Action<T1, T2, T3> task, T1 p1, T2 p2, T3 p3 )
		{
			lock ( this.Tasks )
				this.Tasks.Add( () => task( p1, p2, p3 ) );
		}

		public void Add<T1, T2, T3, T4>( Action<T1, T2, T3, T4> task, T1 p1, T2 p2, T3 p3, T4 p4 )
		{
			lock ( this.Tasks )
				this.Tasks.Add( () => task( p1, p2, p3, p4 ) );
		}

		private zeroflag.Collections.List<Action> _Tasks;

		/// <summary>
		/// Tasks to be processed in the background.
		/// </summary>
		protected zeroflag.Collections.List<Action> Tasks
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
					foreach ( var worker in this.Workers )
						worker.RunWorkerAsync();
				};
				return list;
			}
		}

		#endregion Tasks


		void backgroundWorker_DoWork( object sender, DoWorkEventArgs e )
		{
			Action task = null;
			while ( this.Tasks.Count > 0 && !this.Cancel )
			{
				lock ( this.Tasks )
				{
					if ( this.Tasks[ 0 ] != null )
						task = this.Tasks[ 0 ];
					this.Tasks.RemoveAt( 0 );
				}
				if ( task != null )
					task();
			}
		}

	}
}
