using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Basics
{
	public abstract class TaskBase : IDisposable
	{
		public void RunAsync()
		{
#if SINGLETHREAD
			this.Run(false);
#else
			ThreadPool.QueueUserWorkItem(new WaitCallback(p => this.Run(true)));
#endif
		}

		public virtual void Run(bool isAsync = false)
		{
			try
			{
				try
				{
					this.RunTask(isAsync);
				}
				finally
				{
					this.Finish(isAsync);
				}
			}
			finally
			{
				this.Cleanup();
			}
		}

		protected void Cleanup()
		{
			this.Dispose();
		}

		protected abstract void RunTask(bool isAsync);

		protected abstract void Finish(bool isAsync);

		public virtual void Dispose()
		{
		}
	}
}
