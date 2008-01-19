#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2008  Thomas "zeroflag" Kraemer
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

using System;

namespace zeroflag.Threading
{
	public static class Do
	{
		/// <summary>
		/// Create an empty task for configuration via []
		/// </summary>
		public static Task Run
		{
			get
			{
				return new Task();
			}
		}

		/// <summary>
		/// Create a new parallel For task.
		/// for (int i = start; i &lt; end; i += step) task(i);
		/// </summary>
		/// <param name="start">Start of the iteration. (first value, i = start)</param>
		/// <param name="end">End of the iteration. (past last value, i &lt; end)</param>
		/// <param name="step">Step of each iteration. (i += step)</param>
		/// <param name="task">The task to be executed for each iteration.</param>
		public static For For(int start, int end, int step, Task<int>.RunHandle task)
		{
			return new For(start, end, step, task);
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
		public static For For(int start, int end, Task<int>.RunHandle task)
		{
			return new For(start, end, task);
		}

		/// <summary>
		/// Create a new parallel For task.
		/// for (int i = 0; i &lt; end; i++) task(i);
		/// </summary>
		/// <param name="end">End of the iteration. (past last value, i &lt; end)</param>
		/// <param name="task">The task to be executed for each iteration.</param>
		public static For For(int end, Task<int>.RunHandle task)
		{
			return new For(end, task);
		}

		/// <summary>
		/// Create a new parallel Foreach task.
		/// foreach(T value in collection) task(value);
		/// </summary>
		/// <param name="collection">The values to iterate over.</param>
		/// <param name="task">The task to execute for each value.</param>
		public static Foreach<T> Foreach<T>(System.Collections.Generic.IEnumerable<T> collection, Task<T>.RunHandle task)
		{
			return new Foreach<T>(collection, task);
		}

	}
}
