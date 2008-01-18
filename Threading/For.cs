using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Threading
{
	public class For : Task<int>
	{
		public For(int start, int end, int step, Task<int>.RunHandle task)
		{
			for (int value = start; value < end; value += step)
			{
				this.and(new For(task, value));
			}
		}
		//public For(int start, int end, int step, Task<int>.RunHandle task, Task.RunHandle afterEach)
		//    : this(start,end,step,task, new	Task(afterEach))
		//{
		//    for (int value = start; value < end; value += step)
		//    {
		//        this.and(new For(task, value).Then(afterEach));
		//    }
		//}

		public For(int start, int end, Task<int>.RunHandle task)
			: this(start, end, 1, task)
		{
		}
		public For(int end, Task<int>.RunHandle task)
			: this(0, end, 1, task)
		{
		}


		int value = -1;
		public For(Task<int>.RunHandle task, int value)
		{
			this.value = value;
			this.Handle = delegate()
			{
				task(value);
			};
		}


		//public For(Task<int>.RunHandle task, int value, int end)
		//{
		//    this.value = value;
		//    if (value < end)
		//        this.Handle = delegate()
		//        {
		//            while (value < end)
		//                task(value++);
		//        };
		//    else
		//        this.Handle = delegate()
		//        {
		//            while (value > end)
		//                task(value--);
		//        };
		//}

		//public override string ToString()
		//{
		//    return this.Handle != null ? (this.GetType().Name + "[" + this.value.ToString() + "]") : this.GetType().Name + (this.Next != null ? "=>" + this.Next.GetType().Name : "");
		//}
	}
}
