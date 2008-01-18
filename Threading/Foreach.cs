using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Threading
{
	public class Foreach<T> : Task<T>
	{
		public Foreach(IEnumerable<T> collection, Task<T>.RunHandle task)
		{
			foreach (T value in collection)
			{
				this.and(new Foreach<T>(task, value));
			}
		}

		public Foreach(Task<T>.RunHandle task, T value)
		{
			this.Handle = delegate()
			{
				task(value);
			};
		}
	}
}
