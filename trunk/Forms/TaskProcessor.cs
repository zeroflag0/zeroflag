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

		public bool Cancel
		{
			get { return this.backgroundWorker.CancellationPending; }
			set
			{
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

		#region Tasks
		public void Add( Action task )
		{
			this.Tasks.Add( task );
		}

		public void Add( params Action[] tasks )
		{
			this.Tasks.Add( tasks );
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
					this.backgroundWorker.RunWorkerAsync();
				};
				return list;
			}
		}

		#endregion Tasks


		void backgroundWorker_DoWork( object sender, DoWorkEventArgs e )
		{
			while ( this.Tasks.Count > 0 && !this.Cancel )
			{
				if ( this.Tasks[ 0 ] != null )
					this.Tasks[ 0 ]();
				this.Tasks.RemoveAt( 0 );
			}
		}

	}
}
