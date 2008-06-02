using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Threading
{
	public class For : Task<int>
	{
		/// <summary>
		/// Create a new parallel For task.
		/// for (int i = start; i &lt; end; i += step) task(i);
		/// </summary>
		/// <param name="start">Start of the iteration. (first value, i = start)</param>
		/// <param name="end">End of the iteration. (past last value, i &lt; end)</param>
		/// <param name="step">Step of each iteration. (i += step)</param>
		/// <param name="task">The task to be executed for each iteration.</param>
		public For(int start, int end, int step, Task<int>.RunHandle task)
		{
			this[start, end, step, task].Dummy();
		}

		private void Dummy()
		{
		}
		//public For(int start, int end, int step, Task<int>.RunHandle task, Task.RunHandle afterEach)
		//    : this(start,end,step,task, new	Task(afterEach))
		//{
		//    for (int value = start; value < end; value += step)
		//    {
		//        this.and(new For(task, value).Then(afterEach));
		//    }
		//}

		/// <summary>
		/// Create a new parallel For task.
		/// for (int i = start; i &lt; end; i++) task(i);
		/// </summary>
		/// <param name="start">Start of the iteration. (first value, i = start)</param>
		/// <param name="end">End of the iteration. (past last value, i &lt; end)</param>
		/// <param name="task">The task to be executed for each iteration.</param>
		public For(int start, int end, Task<int>.RunHandle task)
			: this(start, end, 1, task)
		{
		}

		/// <summary>
		/// Create a new parallel For task.
		/// for (int i = 0; i &lt; end; i++) task(i);
		/// </summary>
		/// <param name="end">End of the iteration. (past last value, i &lt; end)</param>
		/// <param name="task">The task to be executed for each iteration.</param>
		public For(int end, Task<int>.RunHandle task)
			: this(0, end, 1, task)
		{
		}


		int _Value = -1;
		//TODO: do I really need these?
		/// <summary>
		/// Internal value. NOT TESTED! DO NOT USE UNLESS YOU KNOW WHAT YOU'RE DOING!
		/// </summary>
		public int Value
		{
			get { return _Value; }
			set { _Value = value; }
		}
		/// <summary>
		/// Create the actual run task. NOT TESTED! DO NOT USE UNLESS YOU KNOW WHAT YOU'RE DOING!
		/// </summary>
		/// <param name="task"></param>
		/// <param name="value"></param>
		public For(Task<int>.RunHandle task, int value)
		{
			this.Value = value;
			this.Handle = delegate()
			{
				task(this.Value);
			};
		}

		int _Value2 = -1;
		//TODO: do I really need these?
		/// <summary>
		/// Internal value. NOT TESTED! DO NOT USE UNLESS YOU KNOW WHAT YOU'RE DOING!
		/// </summary>
		public int Value2
		{
			get { return _Value2; }
			set { _Value2 = value; }
		}
		/// <summary>
		/// Create the actual run task. NOT TESTED! DO NOT USE UNLESS YOU KNOW WHAT YOU'RE DOING!
		/// </summary>
		/// <param name="task"></param>
		/// <param name="value"></param>
		public For(Task<int, int>.RunHandle task, int value1, int value2)
		{
			this.Value = value1;
			this.Value2 = value2;
			this.Handle = delegate()
			{
				task(this.Value2, this.Value);
			};
		}

		/// <summary>
		/// Create a run task. NOT TESTED! DO NOT USE UNLESS YOU KNOW WHAT YOU'RE DOING!
		/// </summary>
		public For()
		{
		}

		//public For this[int start, int end, int step, Task inner]
		//{
		//    get
		//    {
		//        if (inner == this)
		//            return this;
		//        if (inner is For)
		//            return this[start, end, step, (For)inner];
		//        else
		//        {
		//            for (int value = start; value < end; value += step)
		//            {
		//                this.And(inner);
		//            }
		//            return this;
		//        }
		//    }
		//}

		//public For this[int start, int end, int step, For inner, bool waitForInner]
		//{
		//    get
		//    {
		//        for (int value = start; value < end; value += step)
		//        {
					
		//            this.And(new For();
		//        }
		//        return this;
		//    }
		//}


		//TODO: does this even work?
		/// <summary>
		/// Set up an inner For task. (Only to be used with Do.For or new For())
		/// for (int i = start; i &lt; end; i += step) task(i);
		/// WARNING: Not tested! NO idea what this does.
		/// </summary>
		/// <param name="start">Start of the iteration. (first value, i = start)</param>
		/// <param name="end">End of the iteration. (past last value, i &lt; end)</param>
		/// <param name="step">Step of each iteration. (i += step)</param>
		/// <param name="task">The task to be executed for each iteration.</param>
		public For this[int start, int end, int step, Task<int, int>.RunHandle task]
		{
			get
			{
				for (int value = start; value < end; value += step)
				{
					this.And(new For(task, this.Value2, value));
				}
				return this;
			}
		}

		/// <summary>
		/// Set up a For task. (Only to be used with Do.For or new For())
		/// for (int i = start; i &lt; end; i += step) task(i);
		/// </summary>
		/// <param name="start">Start of the iteration. (first value, i = start)</param>
		/// <param name="end">End of the iteration. (past last value, i &lt; end)</param>
		/// <param name="step">Step of each iteration. (i += step)</param>
		/// <param name="task">The task to be executed for each iteration.</param>
		public For this[int start, int end, int step, Task<int>.RunHandle task]
		{
			get
			{
				for (int value = start; value < end; value += step)
				{
					this.And(new For(task, value));
				}
				return this;
			}
		}

		/// <summary>
		/// Set up a For task. (Only to be used with Do.For or new For())
		/// for (int i = start; i &lt; end; i++) task(i);
		/// </summary>
		/// <param name="start">Start of the iteration. (first value, i = start)</param>
		/// <param name="end">End of the iteration. (past last value, i &lt; end)</param>
		/// <param name="task">The task to be executed for each iteration.</param>
		public For this[int start, int end, Task<int>.RunHandle task]
		{
			get
			{
				return this[start, end, 1, task];
			}
		}

		/// <summary>
		/// Set up a For task. (Only to be used with Do.For or new For())
		/// for (int i = 0; i &lt; end; i++) task(i);
		/// </summary>
		/// <param name="end">End of the iteration. (past last value, i &lt; end)</param>
		/// <param name="task">The task to be executed for each iteration.</param>
		public For this[int end, Task<int>.RunHandle task]
		{
			get
			{
				return this[0, end, 1, task];
			}
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