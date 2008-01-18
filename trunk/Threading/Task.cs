#region License
/*
 * Copyright (c) 2006-2007, Thomas "zeroflag" Kraemer
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without modification, 
 * are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list 
 * of conditions and the following disclaimer. Redistributions in binary form must 
 * reproduce the above copyright notice, this list of conditions and the following 
 * disclaimer in the documentation and/or other materials provided with the distribution.
 * Neither the name of the SAGEngine nor the names of its contributors may be used 
 * to endorse or promote products derived from this software without specific prior 
 * written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
 * GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion

using System;
using System.Threading;
namespace zeroflag.Threading
{
	public class Task
	{
		#region Construction
		static int idcount = 0;
		int id;
		public Task()
		{
			id = idcount++;
		}

		public Task(RunHandle handle)
			: this()
		{
			this.Handle = handle;
		}

		public Task(params Task[] group)
			: this()
		{
			this.And(group);
		}
		public Task(params RunHandle[] group)
			: this()
		{
			this.And(group);
		}

		public static implicit operator Task(RunHandle handle)
		{
			return new Task(handle);
		}

		public static Task Do(Task task)
		{
			return task;
		}
		public static Task Do(RunHandle task)
		{
			return new Task(task);
		}
		#endregion Construction

		#region Structure Properties
		public delegate void RunHandle();

		RunHandle m_Handle;

		public RunHandle Handle
		{
			get { return m_Handle; }
			set { m_Handle = value; }
		}

		Task m_Next;

		public Task Next
		{
			get { return m_Next; }
			set { m_Next = value; }
		}


		Task m_Parallel;

		public Task Parallel
		{
			get { return m_Parallel; }
			set { m_Parallel = value; }
		}

		#endregion Structure Properties

		#region Parallel
		public Task And(Task parallel)
		{
			if (this.Parallel == null)
				this.Parallel = parallel;
			else
				this.Parallel.And(parallel);
			return this;
		}

		public Task And(int index, params Task[] group)
		{
			if (index < group.Length)
				return this.And(group[index]).And(index + 1, group);
			else
				return this;
		}

		public Task And(params Task[] group)
		{
			return this.And(0, group);
		}

		public Task And(int index, params RunHandle[] group)
		{
			if (index < group.Length)
				return this.And(group[index]).And(index + 1, group);
			else
				return this;
		}

		public Task And(params RunHandle[] group)
		{
			return this.And(0, group);
		}

		public Task And(RunHandle parallel)
		{
			return this.And(new Task(parallel));
		}
		public Task and(RunHandle parallel)
		{
			return this.And(new Task(parallel));
		}

		public Task and(Task parallel) { return this.And(parallel); }

		public static Task operator +(Task a, Task b)
		{
			if (a == null) return b;
			else if (b == null) return a;

			return a.And(b);
		}

		public static Task operator +(Task a, RunHandle b)
		{
			if (a == null) return b;
			else if (b == null) return a;

			return a.And(b);
		}
		public static Task operator +(RunHandle a, Task b)
		{
			if (a == null) return b;
			else if (b == null) return a;

			return new Task(a).And(b);
		}
		#endregion Parallel

		#region Sequential
		public Task Then(Task next)
		{
			if (this.Next == null)
				this.Next = next;
			else if (next != null)
				this.Next = next.Then(this.Next);
			return this;
		}

		public Task Then(RunHandle next)
		{
			return this.Then(new Task(next));
		}

		public Task then(RunHandle next)
		{
			return this.Then(new Task(next));
		}

		public Task then(Task next)
		{
			return this.Then(next);
		}

		public Task Delay(int milliseconds)
		{
			return this.Then(new Task(delegate()
			{
				System.Threading.Thread.Sleep(milliseconds);
			}));
		}

		public Task delay(int milliseconds)
		{
			return this.Delay(milliseconds);
		}
		public Task Wait(int milliseconds)
		{
			return this.Delay(milliseconds);
		}
		public Task wait(int milliseconds)
		{
			return this.Delay(milliseconds);
		}

		public static Task operator /(Task first, Task next)
		{
			if (first == null) return next;
			else if (next == null) return first;

			return first.Then(next);
		}
		public static Task operator /(RunHandle first, Task next)
		{
			if (first == null) return next;
			else if (next == null) return first;

			return new Task(first).Then(next);
		}
		public static Task operator /(Task first, RunHandle next)
		{
			if (first == null) return next;
			else if (next == null) return first;

			return first.Then(next);
		}
		#endregion Sequential


		#region Execution

		bool m_Done = false;

		public bool Done
		{
			get { return m_Done && this.Parallel != null ? this.Parallel.Done : true; }
			set { m_Done = value; }
		}

		public Task Run()
		{
#if VERBOSE
			Console.WriteLine(this + " run()");
#endif

			return this.Run(null, null);
		}

		Semaphore working = new Semaphore(1, 1);
#if VERBOSE
		int semCount = 0;
#endif
		int runCount = 0;
		int parallelCount = 0;
		int sequentialCount = 0;
		Task Parent;
		Task Previous;
		public Task Run(Task parent, Task previous)
		{
			if (Interlocked.Increment(ref runCount) < 2)
			{
				working.WaitOne();
#if VERBOSE
				Console.WriteLine(this + " locked " + Interlocked.Increment(ref semCount));
#endif

				Interlocked.Increment(ref parallelCount);
				Interlocked.Increment(ref sequentialCount);
				this.Parent = parent;
				this.Previous = previous;

				this.ThreadRun(this);
				if (this.Parallel != null)
				{
					Interlocked.Increment(ref parallelCount);
					this.Parallel.Run(this, null);
				}
			}
#if VERBOSE
			else
				Console.WriteLine(this + " already running");
#endif
			return this;
		}

		protected void DoExecute()
		{
			if (this.Handle != null)
				this.Handle();
		}

		protected void ThreadRun(Task task)
		{
#if VERBOSE
			Console.WriteLine(this + " executing");
#endif

			ThreadPool.QueueUserWorkItem(delegate(object o)
			{
				try
				{
					if (task != null)
						task.DoExecute();
					//if (finished != null)
					//    finished();
				}
				catch (Exception exc)
				{
					this.Error = exc;
				}
				finally
				{
					this.ParallelFinished();
				}
			});
		}


		protected void ParallelFinished()
		{
#if VERBOSE
			Console.WriteLine(this + " finished parallel " + this.parallelCount);
#endif
			if (Interlocked.Decrement(ref this.parallelCount) <= 0)
			{
				if (this.Next != null)
				{
#if VERBOSE
					Console.WriteLine(this + " running next");
#endif
					this.Next.Run(null, this);
				}
				else
					this.NextFinished();
				//PoolRun(this.Next, NextFinished, null);
			}
#if VERBOSE
			else
				Console.WriteLine(this + " -> " + this.Parallel + " parallel remaining");
#endif
		}

		protected void NextFinished()
		{
#if VERBOSE
			Console.WriteLine(this + " finished next");
#endif
			if (Interlocked.Decrement(ref this.sequentialCount) <= 0)
			{
#if VERBOSE
				Console.WriteLine(this + " finishing up");
#endif
				this.OnFinished();

				if (this.Parent != null)
					this.Parent.ParallelFinished();
				else if (this.Previous != null)
					this.Previous.NextFinished();

#if VERBOSE
				Console.WriteLine(this + " released " + Interlocked.Increment(ref semCount));
#endif
				this.sequentialCount = 0;
				this.parallelCount = 0;
				this.Parent = null;
				this.Previous = null;

				try
				{
					working.Release();
				}
				catch (SemaphoreFullException exc)
				{
					Console.WriteLine(this + ": " + exc);
				}
#if VERBOSE
				Console.WriteLine(this + " done");
#endif
			}
		}

		public Task Join()
		{
			return this.Join(Timeout.Infinite);
		}
		public Task Join(int timeout)
		{
#if VERBOSE
			Console.WriteLine(this + " joining...");
#endif
#if VERBOSE
			Console.WriteLine(this + " locked " + Interlocked.Increment(ref semCount));
#endif
			if (working.WaitOne(timeout, false))
			{
				try
				{
#if VERBOSE
					Console.WriteLine(this + " join done");
#endif
					return this;
				}
				finally
				{
#if VERBOSE
					Console.WriteLine(this + " released " + Interlocked.Increment(ref semCount));
#endif
					working.Release();
				}
			}
			return this;
		}

		#region event Done
		private event RunHandle m_Finished;
		/// <summary>
		/// When the task is finished...
		/// </summary>
		public event RunHandle Finished
		{
			add { this.m_Finished += value; }
			remove { this.m_Finished -= value; }
		}
		/// <summary>
		/// Call to raise the Done event:
		/// When the task is finished...
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnFinished()
		{
			// if there are event subscribers...
			if (this.m_Finished != null)
			{
				// call them...
				this.m_Finished();
			}
		}
		#endregion event Done

		#endregion

		#region Error
		Exception m_Error;

		public Exception Error
		{
			get { return m_Error; }
			protected set
			{
				m_Error = value;
				Console.WriteLine(this.ToString() + "\n" + value.ToString());
			}
		}

		public class ExceptionBox
		{
			public ExceptionBox(Task owner, Exception error)
			{
				this.Owner = owner;
				this.Error = error;
			}

			Exception m_Error;

			public Exception Error
			{
				get { return m_Error; }
				set { m_Error = value; }
			}

			Task m_Owner;

			public Task Owner
			{
				get { return m_Owner; }
				set { m_Owner = value; }
			}

			public override string ToString()
			{
				return this.ToString(new System.Text.StringBuilder()).ToString();
			}

			public System.Text.StringBuilder ToString(System.Text.StringBuilder builder)
			{
				return builder.Append(this.Owner.ToString()).Append(": ").Append(this.Error.ToString());
			}
		}

		public System.Collections.Generic.List<ExceptionBox> ErrorsRecursive(System.Collections.Generic.List<ExceptionBox> list)
		{
			if (list == null)
				list = new System.Collections.Generic.List<ExceptionBox>();
			if (this.Error != null)
				list.Add(new ExceptionBox(this, this.Error));
			if (this.Parallel != null)
				this.Parallel.ErrorsRecursive(list);
			if (this.Next != null)
				this.Next.ErrorsRecursive(list);
			return list;
		}

		public string ErrorsRecursive()
		{
			System.Collections.Generic.List<ExceptionBox> exceptions = this.ErrorsRecursive(new System.Collections.Generic.List<ExceptionBox>());
			if (exceptions.Count <= 0)
				return "";

			System.Text.StringBuilder builder = new System.Text.StringBuilder(this.ToString()).Append(" Errors:\n");

			foreach (ExceptionBox exc in exceptions)
			{
				builder.Append(exc.ToString()).AppendLine();
			}

			return builder.AppendLine().ToString();
		}


		#endregion

		public override string ToString()
		{
			return this.GetType().Name + id + "[" + this.ParallelDepth + (this.Parallel != null ? ":" + this.Parallel.ToString(false) : "") + "]=>" + this.SequentialDepth + ":" + (this.Next != null ? this.Next.ToString(false) : "");
		}

		public string ToString(bool recursive)
		{
			if (recursive)
				return this.ToString();
			else
				return this.GetType().Name + id + "[" + this.ParallelDepth + "]=>" + this.SequentialDepth + ":" + (this.Next != null ? this.Next.GetType().Name : "");
		}

		int ParallelDepth
		{
			get { return 1 + (this.Parallel != null ? this.Parallel.ParallelDepth : 0); }
		}
		int SequentialDepth
		{
			get { return 1 + (this.Next != null ? this.Next.SequentialDepth : 0); }
		}
	}
}
