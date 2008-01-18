using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Threading
{
	public class Task<T1> : Task
	{
		public new delegate void RunHandle(T1 i);

		public Task(RunHandle task, T1 v1)
		{
			this.Handle = delegate()
			{
				task(v1);
			};
		}

		public Task()
		{
		}
	}
}
