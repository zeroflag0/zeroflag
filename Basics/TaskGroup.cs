using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Basics
{
	public class TaskGroup : TaskBase
	{
		public TaskGroup()
		{
		}
		public TaskGroup(Action action, params Action[] actions)
			: this(null, action, actions)
		{ 
		}
		public TaskGroup(Action finish, Action action, Action[] actions)
			: this()
		{
			if (finish != null)
				this.FinishAction += t => finish();

			this.Tasks.Add(new Task(action));
			if (actions != null && actions.Length != 0)
				foreach (Action act in actions)
				{
					this.Tasks.Add(new Task(act));
				}
		}

		List<TaskBase> _Tasks = new List<TaskBase>();
		public List<TaskBase> Tasks
		{
			get { return _Tasks; }
		}

		public event Action<TaskBase> FinishAction;

		public override void Run(bool isAsync = false)
		{
			this.RunTask(isAsync);
		}

		protected override void RunTask(bool isAsync)
		{
			Interlocked.Increment(ref _RunningTasks);
			foreach (TaskBase task in this.Tasks)
			{
				this.ExecuteTask(task);
			}
			this.FinishTask(this);
		}

		private void ExecuteTask(TaskBase task)
		{
			Interlocked.Increment(ref _RunningTasks);
#if SINGLETHREAD
			task.Run();
			this.FinishTask(task);
#else
			ThreadPool.QueueUserWorkItem(new WaitCallback(p =>
			{
				task.Run();
				this.FinishTask(task);
			}));
#endif
		}

		int _RunningTasks;
		private void FinishTask(TaskBase task)
		{
			if (Interlocked.Decrement(ref _RunningTasks) != 0)
				return;
#if SINGLETHREAD
			try
			{
				this.Finish(true);
			}
			finally
			{
				this.Cleanup();
			}
#else
			ThreadPool.QueueUserWorkItem(new WaitCallback(p =>
			{
				try
				{
					this.Finish(true);
				}
				finally
				{
					this.Cleanup();
				}
			}));
#endif
		}

		protected override void Finish(bool isAsync)
		{
			if (this.FinishAction != null)
				this.FinishAction(this);
		}

		public override void Dispose()
		{
			base.Dispose();

			this.FinishAction = null;
			this.Tasks.Clear();
			this._Tasks = null;
		}
	}
}
