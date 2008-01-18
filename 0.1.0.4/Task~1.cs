using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Threading
{
	public class Task<T1> : Task
	{
		public new delegate void RunHandle(T1 p1);

		public Task(RunHandle task, T1 p1)
		{
			this.Handle = delegate()
			{
				task(p1);
			};
		}

		public Task()
		{
		}
	}

}
