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

			new For(0, 10, 1, delegate(int i)
			{
				Console.WriteLine(i);
				System.Threading.Thread.Sleep(200);
				Console.WriteLine(i);
			}
				).then(delegate() { Console.WriteLine("All Done"); }).Run().Join();
			Console.WriteLine("Mainthread returned");
			//System.Threading.Thread.Sleep(2000);
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
