#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2007  Thomas "zeroflag" Kraemer
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
using System.Collections.Generic;
using System.Text;
using zeroflag.Threading;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			//new Run(delegate() { Console.WriteLine("test1"); }).then(delegate() { Console.WriteLine("test2"); }).Execute();
			//new Run(delegate() { Console.WriteLine("test1.0"); }).and(delegate() { Console.WriteLine("test1.1"); }).then(delegate() { Console.WriteLine("test2"); }).Execute().Join();

			//Run task1 = new Run(delegate() { System.Threading.Thread.Sleep(100); Console.WriteLine("Task1[Thread={0}]", System.Threading.Thread.CurrentThread.ManagedThreadId); });
			//Run task2 = new Run(delegate() { Console.WriteLine("Task2[Thread={0}]", System.Threading.Thread.CurrentThread.ManagedThreadId); });
			//task1.and(task1).then(task2).Execute().Join();

			for (int r = 0; r < 10; r++)
			{
				Do.Run[delegate
					{
						Console.WriteLine("Start " + r);
					}
					].Then.For[0, 10, 1, delegate(int i)
					{
						Console.WriteLine("> " + r + " " + i);
						System.Threading.Thread.Sleep(100);
						Console.WriteLine("< " + r + " " + i);
					}
					].Then[delegate() { Console.WriteLine("All Done " + r); }].Run().Join();
				//Console.WriteLine("Finished " + r);
			}
			Console.WriteLine("Mainthread returned");
			//System.Threading.Thread.Sleep(2000);

			//Do.Run[delegate
			//{
			//    Console.WriteLine("Start");
			//}].Then.For[0, 10, 1, delegate(int i)
			//    {
			//        Console.WriteLine("> " + i);
			//        System.Threading.Thread.Sleep(200);
			//        Console.WriteLine("< " + i);
			//    }].Then[delegate
			//    {
			//        Console.WriteLine("Done");
			//    }
			//    ].Run().Join();
		}
		static int count = 0;
		static Task Task
		{
			get
			{
				return new For(count++, delegate(int i)
				   {
					   Console.WriteLine(i);
				   });
			}
		}
	}
}
