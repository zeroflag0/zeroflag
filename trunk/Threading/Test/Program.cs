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

//#define TEST_TASKS
#define TEST_MULTIWRITERQUEUE
//#define TEST_VERBOSE_MULTIWRITERQUEUE
#define TEST_PERFORMANCE_MULTIWRITERQUEUE

using System;
using System.Collections.Generic;
using System.Text;
using zeroflag.Threading;

namespace Test
{
	class Program
	{
		static void Main( string[] args )
		{
#if TEST_TASKS
			//new Run(delegate() { Console.WriteLine("test1"); }).then(delegate() { Console.WriteLine("test2"); }).Execute();
			//new Run(delegate() { Console.WriteLine("test1.0"); }).and(delegate() { Console.WriteLine("test1.1"); }).then(delegate() { Console.WriteLine("test2"); }).Execute().Join();

			//Run task1 = new Run(delegate() { System.Threading.Thread.Sleep(100); Console.WriteLine("Task1[Thread={0}]", System.Threading.Thread.CurrentThread.ManagedThreadId); });
			//Run task2 = new Run(delegate() { Console.WriteLine("Task2[Thread={0}]", System.Threading.Thread.CurrentThread.ManagedThreadId); });
			//task1.and(task1).then(task2).Execute().Join();

			for ( int r = 0; r < 100; r++ )
			{
				Do.Run[ delegate
					{
						Console.WriteLine( "Start " + r );
					}
					].Then.For[ 0, 100, 1, delegate( int i )
					{
						//Console.WriteLine("> " + r + " " + i);
						System.Threading.Thread.Sleep( 10 );
						//Console.WriteLine("< " + r + " " + i);
					}
					].Then[ delegate() { Console.WriteLine( "All Done " + r ); } ].Run().Join();
				//Console.WriteLine("Finished " + r);
			}
			Console.WriteLine( "Mainthread returned" );
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
#endif//TEST_TASKS
#if TEST_MULTIWRITERQUEUE
			TestMultiWriterQueue();
#endif//TEST_MULTIWRITERQUEUE
		}
		static int count = 0;
		static Task Task
		{
			get
			{
				return new For( count++, delegate( int i )
				   {
					   Console.WriteLine( i );
				   } );
			}
		}




		class Box<T>
		{
			public T Value;
			//public static implicit operator Box<T>( T value )
			//{
			//    return new Box<T>() { Value = value };
			//}
			//public static implicit operator T( Box<T> box )
			//{
			//    return ( box ?? new Box<T>() ).Value;
			//}
			public override string ToString()
			{
				return this.Value.ToString();
			}
		}
		const string BuildType =
#if DEBUG
 "Debug";
#else
 "Release";
#endif
#if TEST_VERBOSE_MULTIWRITERQUEUE
		const int ItemCount = 20;
		const int ThreadCount = 1;
#else
#if !TEST_PERFORMANCE_MULTIWRITERQUEUE
		const int ItemCount = 2000;
		const int ThreadCount = 4;
#else
		const int ItemCount = 1000000;
		const int ThreadCount = 10;
#endif
#endif
		static void TestMultiWriterQueue()
		{
			Console.WriteLine( "Testing MultiWriterQueue..." );
			Console.WriteLine( "\tConfiguration: " + BuildType + ", Threads=" + ThreadCount + ", Items=" + ItemCount + ")" );
			Console.WriteLine( "\tSystem: " + Environment.OSVersion + ", CPUs=" + Environment.ProcessorCount );
			zeroflag.Threading.MultiWriterQueue<Box<KeyValuePair<int, int>>> queue = new MultiWriterQueue<Box<KeyValuePair<int, int>>>();

			int threadsFinished = 0;
			for ( int t = 0; t < ThreadCount; t++ )
				new System.Threading.Thread( () =>
					{
						Console.WriteLine( System.Threading.Thread.CurrentThread.ManagedThreadId + "> Filling queue..." );
						System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
						sw.Start();
						Random rand = new Random();
						for ( int i = 0; i < ItemCount; i++ )
						{
							queue.Write( new Box<KeyValuePair<int, int>>() { Value = new KeyValuePair<int, int>( System.Threading.Thread.CurrentThread.ManagedThreadId, i ) } );
#if TEST_VERBOSE_MULTIWRITERQUEUE
							Console.WriteLine( System.Threading.Thread.CurrentThread.ManagedThreadId + "> wrote " + queue.First );
#endif
							//System.Threading.Interlocked.Increment( ref count );
#if !TEST_PERFORMANCE_MULTIWRITERQUEUE
							System.Threading.Thread.Sleep( rand.Next( 10 ) );
#endif
						}
						sw.Stop();
						Console.WriteLine( System.Threading.Thread.CurrentThread.ManagedThreadId + "> Wrote " + ItemCount + " items in " + sw.ElapsedMilliseconds + "ms." );
						System.Threading.Interlocked.Increment( ref threadsFinished );
					} ).Start();
			Dictionary<int, int> results = new Dictionary<int, int>();

			//while ( threadsFinished != ThreadCount )
			//    System.Threading.Thread.Sleep( 100 );

			int empty = 0;
			int read = 0;
			{
				Console.WriteLine( "Beginning to read..." );
				System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
				sw.Start();
				do
				{
					var node = queue.First;
					var box = queue.Read();
					if ( box != null )
					{
						empty = 0;
						//System.Threading.Interlocked.Decrement( ref count );
						KeyValuePair<int, int> value = box.Value;
#if TEST_VERBOSE_MULTIWRITERQUEUE
						Console.WriteLine( "  < read " + node );
						Console.WriteLine( "\t" + queue.First );
						Console.WriteLine( "\t" + queue.Last );
#endif
						if ( results.ContainsKey( value.Key ) && results[ value.Key ] != value.Value - 1 )
						{
							if ( results[ value.Key ] < value.Value )
								Console.WriteLine( "Missing value from writer " + value.Key + ": previous=" + results[ value.Key ] + ", current=" + value.Value );
							else
								Console.WriteLine( "Duplicated value from writer " + value.Key + ": previous=" + results[ value.Key ] + ", current=" + value.Value );
						}
						else
							read++;

						results[ value.Key ] = value.Value;
					}
					else
					{
						if ( read < ThreadCount * ItemCount )
						{
							empty++;
							if ( threadsFinished == ThreadCount )
							{
								if ( empty > ItemCount / 2 )
								{
									Console.WriteLine( "Could not read all items..." );
									Console.WriteLine( "\tFirst = " + ( queue.First ?? (object)"<null>" ).ToString() );
									Console.WriteLine( "\tLast  = " + ( queue.Last ?? (object)"<null>" ).ToString() );
									break;
								}
								//zeroflag.Threading.MultiWriterQueue<Box<KeyValuePair<int, int>>>.Node first = queue.First;
								//zeroflag.Threading.MultiWriterQueue<Box<KeyValuePair<int, int>>>.Node last = queue.Last;
							}
							System.Threading.Thread.Sleep( 10 );
							continue;
						}
						else
							break;


						//Console.WriteLine( "Read null while IsEmpty=" + queue.IsEmpty );
					}
				}
				while ( true );
				sw.Stop();
				Console.WriteLine( read + " items from " + ThreadCount + " threads received in " + sw.ElapsedMilliseconds + "ms (" + (int)( read / ( sw.ElapsedMilliseconds / 1000.0 ) ) + " messages/second)." );
			}
		}
	}
}
