#define DEBUG_TRACE

#region LGPL License

//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag >>at<< zeroflag >>dot<< de
//	
//	Copyright (C) 2006-2009  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************

#endregion LGPL License

#region BSD License

/*
 * Copyright (c) 2006-2008, Thomas "zeroflag" Kraemer
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

#endregion BSD License

using System;
using System.Diagnostics;
using System.Threading;
using zeroflag.Collections;
using zeroflag.Threading;
//using System.Collections.Generic;

namespace zeroflag.Components
{
    public partial class TaskProcessor
        : Component,
            ITaskProcessor
    {
        public TaskProcessor()
        {
            this.InitializeComponent();
        }

        public TaskProcessor(Component outer)
        {
            this.Outer = outer;
            this.InitializeComponent();
        }

#if !SILVERLIGHT
		public TaskProcessor(System.ComponentModel.IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}
#endif

        #region Thread

        private Thread _Thread;

        /// <summary>
        /// The thread used by this TaskProcessor.
        /// </summary>
        public Thread Thread
        {
            get { return this._Thread ?? (this._Thread = this.ThreadCreate); }
            protected set
            {
                if (this._Thread != value)
                {
                    //if (_Thread != null) { }
                    this._Thread = value;
                    //if (_Thread != null) { }
                }
            }
        }

        /// <summary>
        /// Creates the default/initial value for Thread.
        /// The thread used by this TaskProcessor.
        /// </summary>
        protected virtual Thread ThreadCreate
        {
            get
            {
                var value = new Thread(this.Run);
#if !SILVERLIGHT
				value.SetApartmentState(System.Threading.ApartmentState.STA);
#endif
                value.Start();
                return value;
            }
        }

        #region ThreadPriority

#if !SILVERLIGHT
		private System.Threading.ThreadPriority _ThreadPriority = System.Threading.ThreadPriority.Normal;

		/// <summary>
		/// The thread's priority.
		/// </summary>
		public System.Threading.ThreadPriority ThreadPriority
		{
			get
			{
				var thread = this._Thread;
				if (thread != null)
				{
					_ThreadPriority = thread.Priority;
				}
				return _ThreadPriority;
			}
			set
			{
				if (_ThreadPriority != value)
				{
					_ThreadPriority = value;
					var thread = this._Thread;
					if (thread != null)
					{
						thread.Priority = value;
					}
				}
			}
		}
#endif

        #endregion ThreadPriority

        #endregion Thread

        #region Disposing

        private bool _Disposing;

        /// <summary>
        /// When this processor is disposing.
        /// </summary>
        public bool Disposing
        {
            get { return this._Disposing; }
            set
            {
                if (this._Disposing != value)
                {
#if DEBUG
                    if (this._Disposing && !value)
                    {
                        Console.WriteLine((this.Name ?? this.ToString()) + ".Disposing = " + value);
                        Console.WriteLine(new StackTrace(new StackFrame(1)));
                    }
#endif
                    this.OnDisposingChanged(this._Disposing, this._Disposing = value);
                }
            }
        }

        #region DisposingChanged event

        public delegate void DisposingChangedHandler(object sender, bool oldvalue, bool newvalue);

        private event DisposingChangedHandler _DisposingChanged;

        /// <summary>
        /// Occurs when Disposing changes.
        /// </summary>
        public event DisposingChangedHandler DisposingChanged
        {
            add { this._DisposingChanged += value; }
            remove { this._DisposingChanged -= value; }
        }

        /// <summary>
        /// Raises the DisposingChanged event.
        /// </summary>
        protected virtual void OnDisposingChanged(bool oldvalue, bool newvalue)
        {
            // if there are event subscribers...
            if (this._DisposingChanged != null)
            {
                // call them...
                this._DisposingChanged(this, oldvalue, newvalue);
            }
        }

        #endregion DisposingChanged event

        #endregion Disposing

        private bool _Cancel;

        public bool Cancel
        {
            get { return this._Cancel; }
            set
            {
                if (value != this._Cancel)
                {
#if DEBUG && false
					Console.WriteLine( ( this.Name ?? this.ToString() ) + ".Cancel = " + value );
					Console.WriteLine( new System.Diagnostics.StackTrace( 1 ) );
#endif
                    this._Cancel = value;
                    if (this._Cancel)
                    {
                        this.CancelRequestTime = Time.Now;
                    }
                    else
                    {
                        this.CancelRequestTime = null;
                    }
                }
                //Console.WriteLine( this + ".Cancel = " + value );
                //if ( value )
                //{
                //    this.Tasks.Clear();
                //    this.backgroundWorker.CancelAsync();
                //}
                //else
                //this.backgroundWorker.RunWorkerAsync();
            }
        }

        public bool IsRunning
        {
            get { return this._Thread != null && this.Thread.IsAlive && this._Working > 0; }
        }

        #region Parentless

        private bool _Parentless;

        /// <summary>
        /// Whether the TaskProcessor can run without a parent(outer/core) object. If false(default) it will exit when missing a parent.
        /// </summary>
        public bool Parentless
        {
            get { return this._Parentless; }
            set
            {
                if (this._Parentless != value)
                {
                    this._Parentless = value;
                }
            }
        }

        #endregion Parentless

        #region Name

        private string _Name;

        /// <summary>
        /// This taskprocessor's name (optional).
        /// </summary>
        public string Name
        {
            get { return this._Name; }
            set
            {
                if (this._Name != value)
                {
                    this._Name = value;
                    this._Restart = 1;
                    //if ( _Tasks != null )
                    //    this.Add( () => System.System.Threading.Thread.CurrentThread.Name = value );
                }
            }
        }

        #endregion Name

        #region event ErrorHandling

        public delegate void ErrorHandlingHandler(Task task, Exception exc);

        private event ErrorHandlingHandler _ErrorHandling;

        /// <summary>
        /// When an error occurs during task execution.
        /// </summary>
        public event ErrorHandlingHandler ErrorHandling
        {
            add { this._ErrorHandling += value; }
            remove { this._ErrorHandling -= value; }
        }

        /// <summary>
        /// Call to raise the ErrorHandling event:
        /// When an error occurs during task execution.
        /// </summary>
        protected virtual void OnErrorHandling(Task task, Exception exc)
        {
            // if there are event subscribers...
            if (this._ErrorHandling != null)
            {
                // call them...
                this._ErrorHandling(task, exc);
            }
        }

        #endregion event ErrorHandling

        #region Tasks

        public void Add(Task task)
        {
            this.Tasks.Add(task);
            this.Thread.GetType();
            //            if ( !this.backgroundWorker.IsBusy || _Working < 1 )
            //            {
            //                try
            //                {
            //                    this.backgroundWorker.RunWorkerAsync();
            //                }
            //#if DEBUG
            //                            //|| TRACE || VERBOSE
            //                        catch ( Exception exc )
            //                        {
            //                            Console.WriteLine( exc );
            //                        }
            //#else
            //                catch { }
            //#endif
            //            }
            this.Wait.Set();
        }

        public void Add(params Action[] tasks)
        {
            foreach (Action task in tasks)
            {
                this.Add(Task.Create(task));
            }
        }

        public void Add<T1>(Action<T1> task, T1 p1)
        {
            this.Add(Task.Create(task, p1));
        }

        public void Add<T1, T2>(Action<T1, T2> task, T1 p1, T2 p2)
        {
            this.Add(Task.Create(task, p1, p2));
        }

        public void Add<T1, T2, T3>(Action<T1, T2, T3> task, T1 p1, T2 p2, T3 p3)
        {
            this.Add(Task.Create(task, p1, p2, p3));
        }

        public void Add<T1, T2, T3, T4>(Action<T1, T2, T3, T4> task, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            this.Add(Task.Create(task, p1, p2, p3, p4));
        }

        public void Add(DateTime time, params Action[] tasks)
        {
            foreach (Action task in tasks)
            {
                this.Add(Task.Create(time, task));
            }
        }

        public void Add<T1>(DateTime time, Action<T1> task, T1 p1)
        {
            this.Add(Task.Create(time, task, p1));
        }

        public void Add<T1, T2>(DateTime time, Action<T1, T2> task, T1 p1, T2 p2)
        {
            this.Add(Task.Create(time, task, p1, p2));
        }

        public void Add<T1, T2, T3>(DateTime time, Action<T1, T2, T3> task, T1 p1, T2 p2, T3 p3)
        {
            this.Add(Task.Create(time, task, p1, p2, p3));
        }

        public void Add<T1, T2, T3, T4>(DateTime time, Action<T1, T2, T3, T4> task, T1 p1, T2 p2, T3 p3, T4 p4)
        {
            this.Add(Task.Create(time, task, p1, p2, p3, p4));
        }

        public int Count
        {
            get { return this.Tasks.Count; }
        }

        public void Clear()
        {
            this.Tasks.Clear();
        }

        private LocklessQueue<Task?> _Tasks;

        /// <summary>
        /// Tasks to be processed in the background.
        /// </summary>
        protected LocklessQueue<Task?> Tasks
        {
            get { return this._Tasks ?? (this._Tasks = this.TasksCreate); }
        }

        /// <summary>
        /// Creates the default/initial value for Tasks.
        /// Tasks to be processed in the background.
        /// </summary>
        protected virtual LocklessQueue<Task?> TasksCreate
        {
            get
            {
                LocklessQueue<Task?> list = this._Tasks = new LocklessQueue<Task?> { };
#if OBSOLETE
				list.ItemAdded += item =>
				{
					if ( !this.backgroundWorker.IsBusy || _Working < 1 )
					{
						try
						{
							this.backgroundWorker.RunWorkerAsync();
						}
#if DEBUG
							//|| TRACE || VERBOSE
						catch ( Exception exc )
						{
							Console.WriteLine( exc );
						}
#else
						catch {}
#endif
					}
					Wait.Set();
				};
#endif
                //if ( _Name != null )
                //    this.Add( () => System.Threading.Thread.CurrentThread.Name = this.Name );
                return list;
            }
        }

        #region ScheduledTasks

        private List<Task?> _ScheduledTasks;

        /// <summary>
        /// The tasks scheduled for a specific time, in order of scheduled time.
        /// </summary>
        public List<Task?> ScheduledTasks
        {
            get { return this._ScheduledTasks ?? (this._ScheduledTasks = this.ScheduledTasksCreate); }
            //protected set
            //{
            //	if (_ScheduledTasks != value)
            //	{
            //		//if (_ScheduledTasks != null) { }
            //		_ScheduledTasks = value;
            //		//if (_ScheduledTasks != null) { }
            //	}
            //}
        }

        /// <summary>
        /// Creates the default/initial value for ScheduledTasks.
        /// The tasks scheduled for a specific time, in order of scheduled time.
        /// </summary>
        protected virtual List<Task?> ScheduledTasksCreate
        {
            get
            {
                List<Task?> value = this._ScheduledTasks = new List<Task?>();
                value.ItemAdded += this.ScheduledTasksAdded;
                return value;
            }
        }

        private void ScheduledTasksAdded(Task? item)
        {
            this._ScheduledTasks.Sort((a, b) => a.Value.Time.CompareTo(b.Value.Time));
        }

        #endregion ScheduledTasks

        #endregion Tasks

        #region CancelTimeout

        #region CancelRequestTime

        private Time? _CancelRequestTime;

        /// <summary>
        /// When was the Cancel issued?
        /// </summary>
        public Time? CancelRequestTime
        {
            get { return this._CancelRequestTime; }
            set
            {
                if (this._CancelRequestTime != value)
                {
                    this._CancelRequestTime = value;
                }
            }
        }

        #endregion CancelRequestTime

        private Time _CancelTimeout = 2000;

        /// <summary>
        /// Time for how long the thread will be allowed to contineue after Cancel has been set.
        /// </summary>
        public Time CancelTimeout
        {
            get { return this._CancelTimeout; }
            set
            {
                if (this._CancelTimeout != value)
                {
                    this._CancelTimeout = value;
                }
            }
        }

        #endregion CancelTimeout

        #region IdleThreadTimeout

        private TimeSpan _IdleThreadTimeout = TimeSpan.FromSeconds(2.0);

        /// <summary>
        /// Time after which the worker thread gets suspended (leaves active waiting).
        /// </summary>
        public TimeSpan IdleThreadTimeout
        {
            get { return this._IdleThreadTimeout; }
            set
            {
                if (this._IdleThreadTimeout != value)
                {
                    this._IdleThreadTimeout = value;
                }
            }
        }

        #endregion IdleThreadTimeout

#if DEBUG_TRACE

        #region DebugTrace

        private static bool _DebugTrace;

        /// <summary>
        /// Trace the current state of the worker.
        /// </summary>
        public static bool DebugTrace
        {
            get { return _DebugTrace; }
            set
            {
                if (_DebugTrace != value)
                {
                    _DebugTrace = value;
                    if (value)
                    {
                        lock (_WaitHandles)
                        {
                            foreach (AutoResetEvent handle in _WaitHandles)
                            {
                                handle.Set();
                            }
                        }
                    }
                }
            }
        }

        #endregion DebugTrace

        private static readonly List<AutoResetEvent> _WaitHandles = new List<AutoResetEvent>();
#endif

        //System.Threading.AutoResetEvent Wait = new System.Threading.AutoResetEvent( false );

        #region Wait

        private AutoResetEvent _Wait;

        /// <summary>
        /// Wait handle.
        /// </summary>
        protected AutoResetEvent Wait
        {
            get { return this._Wait ?? (this._Wait = this.WaitCreate); }
            //set { _Wait = value; }
        }

        /// <summary>
        /// Creates the default/initial value for Wait.
        /// Wait handle.
        /// </summary>
        protected virtual AutoResetEvent WaitCreate
        {
            get
            {
                AutoResetEvent Wait = this._Wait = new AutoResetEvent(false);
#if DEBUG
                lock (_WaitHandles)
                {
                    _WaitHandles.Add(this._Wait);
                }
#endif
                return Wait;
            }
        }

        #endregion Wait

        private Time _LastWork = Time.Now;
        private int _MissingParent;
        private int _Working;
        private int _Restart;
        //void backgroundWorker_DoWork( object sender, DoWorkEventArgs e )
        protected virtual void Run()
        {
            if (Interlocked.CompareExchange(ref this._Working, 1, 0) == 0)
            {
                if (this._Name != null)
                {
                    try
                    {
                        Thread.CurrentThread.Name = this.Name;
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);
                    }
                }
#if !SILVERLIGHT
				if (this.ThreadPriority !=
				    System.Threading.Thread.CurrentThread.Priority)
				{
					try
					{
						System.Threading.Thread.CurrentThread.Priority = this.ThreadPriority;
					}
					catch (Exception exc)
					{
						Console.WriteLine(exc);
					}
				}
#endif
                try
                {
                    Task? current;
                    //System.Threading.Interlocked.Increment( ref _Working );
                    //if ( _Working > 1 )
                    //    return;
                    Console.WriteLine("TaskProcessor(" + this.Name + ") started...");
                    while (!this.Cancel ||
                        !this.Tasks.IsEmpty)
                    {
                        if (!this.Parentless && this.Outer == null && this.CoreBase == null
#if !SILVERLIGHT
						    && this.Site == null
#endif
                            )
                        {
                            if (this._MissingParent++ > 10)
                            {
                                Console.WriteLine("TaskProcessor(" + this.Name + ") exiting, missing parent...");
                                break;
                            }
                        }
                        else
                        {
                            this._MissingParent = 0;
                        }
                        if (Interlocked.CompareExchange(ref this._Restart, 0, 1) != 0)
                        {
                            Console.WriteLine("TaskProcessor(" + this.Name + ") restarting...");
                            this.Thread = null;
                            this._Working = 0;
                            this.Thread = this.ThreadCreate;
                            this.Wait.Set();
                            return;
                        }
                        if ((this.Cancel || this.Disposing) &&
                            (Time.Now - this.CancelRequestTime) >
                                this.CancelTimeout * 0.85)
                        {
                            Console.WriteLine("TaskProcessor(" + this.Name + ") canceled...");
                            break;
                        }
                        if (this.Tasks.Count > 0 || this._ScheduledTasks.Count > 0)
                        {
                            //current = this.Tasks[0];
                            current = null;
                            if (this.ScheduledTasks.Count > 0)
                            {
                                current = this.ScheduledTasks[0];
                                //verbose( "Testing " + current.Value );
                                if (current.Value.Time < Time.Now)
                                {
                                    verbose("Using " + current);
                                    this.ScheduledTasks.RemoveAt(0);
                                }
                                else
                                {
                                    current = null;
                                }
                            }
                            current = current ?? this.Tasks.Read();
                            this._Current = current;
                            if (current != null)
                            {
                                verbose("Current = " + current);
                                if (current.Value.Time == DateTime.MinValue || current.Value.Time < Time.Now)
                                {
                                    try
                                    {
                                        verbose("Executing " + current);
                                        current.Value.Action();
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine(exc);
                                        this.OnErrorHandling(current.Value, exc);
                                    }
                                }
                                else
                                {
                                    verbose("Scheduling " + current);
                                    this.ScheduledTasks.Add(current);
                                    this.ScheduledTasks.Sort((a, b) => a.Value.Time.CompareTo(b.Value.Time));
                                }
                            }
                            this._LastWork = Time.Now;
                            //this.Tasks.RemoveAt( 0 );
                        }
                        else
                        {
                            if (Time.Now - this._LastWork > this.IdleThreadTimeout &&
                                !this.Cancel)
                            {
#if DEBUG
                                //Console.WriteLine( "TaskProcessor(" + this.Name + ") idle timeout." );
#endif
                                //this.Wait.WaitOne( this.IdleThreadTimeout, true );
                                this.Wait.WaitOne(this.IdleThreadTimeout);
#if DEBUG
                                //Console.WriteLine( "TaskProcessor(" + this.Name + ") resuming..." );
#endif
                                this._LastWork = Time.Now;
                                //return;
                            }
                            else
                            {
                                //this.Wait.WaitOne( 200, true );
                                this.Wait.WaitOne(200);
                            }
                        }
#if DEBUG_TRACE
                        if (DebugTrace)
                        {
                            Console.WriteLine(this + (this.Cancel ? " Cancel" : "") + (this.Disposing ? " Disposing" : "") + " \n\touter="
                                + (this.Outer == null ? "<null>" : this.Outer + " " + this.Outer.State) + " \n\tcore="
                                    + (this.CoreBase == null ? "<null>" : this.CoreBase + " " + this.CoreBase.State) + " \n\t" + Time.Now + " - "
                                        + this.Tasks.Count + ": \n\t" + this.Current);
                            //Console.WriteLine( new System.Diagnostics.StackTrace() );
                        }
#endif
                    }
                    Console.WriteLine("TaskProcessor(" + this.Name + ") halted.");
                }
                catch (ThreadAbortException)
                {
                    Console.WriteLine("TaskProcessor(" + this.Name + ") aborted!");
                }
                finally
                {
                    //System.Threading.Interlocked.Decrement( ref _Working );
                    this._Working = 0;
                }
            }
        }

        #region Current

        private Task? _Current;

        /// <summary>
        /// The action currently executing.
        /// </summary>
        public Task? CurrentAction
        {
            get { return this._Current; }
            //set
            //{
            //    if ( _Current != value )
            //    {
            //        _Current = value;
            //    }
            //}
        }

        public string Current
        {
            get
            {
                Task? current = this._Current;
                if (current == null)
                {
                    return "<null>";
                }
                return current.ToString();
            }
        }

        #endregion Current

        public override string ToString()
        {
            return (this.Name ?? base.ToString());
        }

        public virtual void Finish()
        {
            Console.WriteLine(this + ".Finish() " + this.Current);
            this.Cancel = true;
            this.Wait.Set();
        }

        protected override void OnOuterChanged(Component oldvalue, Component newvalue)
        {
            base.OnOuterChanged(oldvalue, newvalue);
            if (newvalue == null)
            {
                Console.WriteLine(this + " Outer is null");
                this.Dispose();
            }
            else
            {
                Console.WriteLine(this + " Outer is " + newvalue);
            }
        }

        protected override void OnDispose()
        {
            Console.WriteLine(this + ".OnDispose()");
            this.Wait.Set();
            this.Disposing = true;
            this.Cancel = true;
            if (Thread.CurrentThread == this.Thread)
            {
                return;
            }
            while (this.IsRunning)
            {
                this.Wait.Set();
                Thread.Sleep(10);
                if (Time.Now - this.CancelRequestTime > this.CancelTimeout)
                {
                    Console.WriteLine((this.Name ?? this.ToString()) + "CancelTimeout");
                    try
                    {
                        this.Thread.Abort();
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc);
                    }
                    break;
                }
            }
            //while ( this.backgroundWorker.IsBusy && _Working > 0 )
            //{
            //    Wait.Set();
            //    System.System.Threading.Thread.Sleep( 50 );
            //    if ( Time.Now - this.CancelRequestTime > this.CancelTimeout )
            //    {
            //        Console.WriteLine( ( this.Name ?? this.ToString() ) + "CancelTimeout" );
            //        try
            //        {
            //            this.backgroundWorker.CancelAsync();
            //        }
            //        catch ( Exception exc )
            //        {
            //            Console.WriteLine( exc );
            //        }
            //        break;
            //    }
            //}
        }


        [Conditional("DEBUG")]
        public static void dbg(object msg)
        {
            Console.WriteLine(msg);
        }

        [Conditional("VERBOSE_TASKPROCESSOR")]
        public static void verbose(object msg)
        {
            msg = Time.Now + " " + msg;
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
        }
    }
}
