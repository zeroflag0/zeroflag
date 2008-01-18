using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Threading
{
	public class Task<T1, T2> : Task
	{
		public new delegate void RunHandle(T1 p1, T2 p2);

		public Task(RunHandle task, T1 p1, T2 p2)
		{
			this.Handle = delegate()
			{
				task(p1, p2);
			};
		}

		public Task()
		{
		}
	}
}
