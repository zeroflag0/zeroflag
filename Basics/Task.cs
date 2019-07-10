using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basics
{
	public class Task : TaskBase
	{
		public Task(Action run, Action finish = null)
			: this(t => run(), finish != null ? (t => finish()) : (Action<TaskBase>)null)
		{
		}

		public Task(Action<TaskBase> run, Action<TaskBase> finish = null)
		{
			this.RunAction += run;
			if (finish != null)
				this.FinishAction += finish;
		}

		public event Action<TaskBase> RunAction;
		public event Action<TaskBase> FinishAction;

		protected override void RunTask(bool isAsync = false)
		{
			this.RunAction(this);
		}

		protected override void Finish(bool isAsync)
		{
			if (this.FinishAction != null)
				this.FinishAction(this);
		}

		public override void Dispose()
		{
			base.Dispose();

			this.RunAction = null;
			this.FinishAction = null;
		}
	}
}
