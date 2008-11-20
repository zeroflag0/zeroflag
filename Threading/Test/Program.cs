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

//#define BIT64		// uses Int64 for testing
//#define BITLARGE		// uses NxInt32 for testing.
#if BITLARGE32 || BITLARGE24 || BITLARGE16
#define BITLARGE
#endif

#if BITLARGE
#if BITLARGE32
#define BITLARGE24
#endif
#if BITLARGE24
#define BITLARGE16
#endif
//#define BITLARGE32
#endif

#define TEST_MULTIWRITERQUEUE
#define TEST_PERFORMANCE_MULTIWRITERQUEUE
#if !TEST_PERFORMANCE_MULTIWRITERQUEUE
//#define TEST_VERBOSE_MULTIWRITERQUEUE
#endif

//#define TEST_SINGLEWRITERQUEUE
#define TEST_PERFORMANCE_SINGLEWRITERQUEUE
#if !TEST_PERFORMANCE_SINGLEWRITERQUEUE
//#define TEST_VERBOSE_SINGLEWRITERQUEUE
#endif

using System;
using System.Collections.Generic;
using System.Text;
using zeroflag.Threading;

namespace Test
{
#if BIT64
	using Data = System.Int64;
#elif BITLARGE
	using Data = TestData;
	struct TestData
	{
		int a0;
		int a1;
		int a2;
		int a3;
		int a4;
		int a5;
		int a6;
		int a7;
		int a8;
		int a9;

#if BITLARGE16
		int b0;
		int b1;
		int b2;
		int b3;
		int b4;
		int b5;
		int b6;
		int b7;
		int b8;
		int b9;

#if BITLARGE24
		int c0;
		int c1;
		int c2;
		int c3;
		int c4;
		int c5;
		int c6;
		int c7;
		int c8;
		int c9;

#if BITLARGE32
		int d0;
		int d1;
		int d2;
		int d3;
		int d4;
		int d5;
		int d6;
		int d7;
		int d8;
		int d9;
#endif
#endif
#endif

		public TestData( int value )
		{
#if BITLARGE16
			b0 = b1 = b2 = b3 = b4 = b5 = b6 = b7 = b8 = b9 =
#if BITLARGE24
			c0 = c1 = c2 = c3 = c4 = c5 = c6 = c7 = c8 = c9 = 
#if BITLARGE32
			d0 = d1 = d2 = d3 = d4 = d5 = d6 = d7 = d8 = d9 =
#endif
#endif
#endif
			a0 = a1 = a2 = a3 = a4 = a5 = a6 = a7 = a8 = a9 = value;
		}

		public static implicit operator TestData( int value )
		{
			return new TestData( value );
		}
		public static implicit operator int( TestData value )
		{
			return value.a0;
		}
	}
#else
	using Data = System.Int32;
#endif
	class Program
	{
		static int DataSize
		{
			get { return System.Runtime.InteropServices.Marshal.SizeOf( typeof( Data ) ); }
		}
		static StringBuilder Report = new StringBuilder();
		static void Main( string[] args )
		{
			Report.Append( "System: " + Environment.OSVersion + " " + ( IntPtr.Size * 8 ) + "bit, CPUs=" + Environment.ProcessorCount + ", CLR=" + Environment.Version + ", ItemSize=" + DataSize + "bytes" ).AppendLine();

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
#if TEST_VERBOSE_MULTIWRITERQUEUE
			TestMultiWriterQueue( 20, 1 );
#else
#if !TEST_PERFORMANCE_MULTIWRITERQUEUE
			TestMultiWriterQueue( 2000, 4 );
#else
			TestMultiWriterQueue( 20, 1 );
			TestMultiWriterQueue( 2000000, 20 );
			//TestMultiWriterQueue( 2000000, 10 );
			TestMultiWriterQueue( 2000000, 8 );
			TestMultiWriterQueue( 2000000, 4 );
			TestMultiWriterQueue( 2000000, 2 );
			TestMultiWriterQueue( 2000000, 1 );
#endif
#endif
#endif//TEST_MULTIWRITERQUEUE

#if TEST_SINGLEWRITERQUEUE
#if TEST_VERBOSE_SINGLEWRITERQUEUE
			TestSingleWriterQueue( 20 );
#else
#if !TEST_PERFORMANCE_SINGLEWRITERQUEUE
			TestSingleWriterQueue( 2000 );
#else
			TestSingleWriterQueue( 20 );
			TestSingleWriterQueue( 2048 );
			TestSingleWriterQueue( 1000000 );
			TestSingleWriterQueue( 10000000 );
#endif
#endif
#endif//TEST_MULTIWRITERQUEUE
			System.IO.File.WriteAllText( "Report" + DataSize + ".txt", Report.ToString() );
			System.Windows.Forms.Application.Run( new ReportForm( Report.ToString() ) );
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
		//const int ItemCount = 20;
		//const int ThreadCount = 1;
		//const int ItemCount = 2000;
		//const int ThreadCount = 4;
		//const int ItemCount = 2000000;
		//const int ThreadCount = 60;
#if TEST_MULTIWRITERQUEUE
		static void TestMultiWriterQueue( int ItemCount, int ThreadCount )
		{
			GC.Collect();
			System.Threading.Thread.Sleep( 10 );
			GC.Collect();

			Console.WriteLine( "Testing MultiWriterQueue..." );
			//Report.Append( "MultiWriterQueue Configuration: " + BuildType + ", Threads=" + ThreadCount + ", Items=" + ItemCount + ", ItemSize=" + System.Runtime.InteropServices.Marshal.SizeOf( typeof( Data ) ) + "bytes, performance test" ).AppendLine();
			Console.WriteLine( "\tConfiguration: " + BuildType + ", Threads=" + ThreadCount + ", Items=" + ItemCount + ", ItemSize=" + System.Runtime.InteropServices.Marshal.SizeOf( typeof( Data ) ) + "bytes, performance test" );
			Console.WriteLine( "\tSystem: " + Environment.OSVersion + ", CPUs=" + Environment.ProcessorCount );
			zeroflag.Threading.LocklessQueue<Box<KeyValuePair<int, Data>>> queue = new LocklessQueue<Box<KeyValuePair<int, Data>>>();
			List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
			int threadsFinished = 0;
			for ( int t = 0; t < ThreadCount; t++ )
				threads.Add(
				new System.Threading.Thread( () =>
				{
					//Console.WriteLine( System.Threading.Thread.CurrentThread.ManagedThreadId + "> Filling queue..." );
					System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
					sw.Start();
					//Random rand = new Random();
					for ( int i = 0; i < ItemCount; i++ )
					{
						queue.Write( new Box<KeyValuePair<int, Data>>() { Value = new KeyValuePair<int, Data>( System.Threading.Thread.CurrentThread.ManagedThreadId, i ) } );
#if TEST_VERBOSE_MULTIWRITERQUEUE
							Console.WriteLine( System.Threading.Thread.CurrentThread.ManagedThreadId + "> wrote " + queue.First );
#endif
						//System.Threading.Interlocked.Increment( ref count );
#if !TEST_PERFORMANCE_MULTIWRITERQUEUE
							System.Threading.Thread.Sleep( rand.Next( 10 ) );
#endif
					}
					sw.Stop();
					Console.WriteLine( System.Threading.Thread.CurrentThread.ManagedThreadId + "> Wrote " + ItemCount + " items(" + DataSize + "bytes each) in " + sw.ElapsedMilliseconds + "ms." );
					//Console.WriteLine( System.Threading.Thread.CurrentThread.ManagedThreadId + "> Wrote " + ItemCount + " items in " + sw.ElapsedMilliseconds + "ms." );
					System.Threading.Interlocked.Increment( ref threadsFinished );
				} ) );
			Dictionary<int, Data> results = new Dictionary<int, Data>();

			//while ( threadsFinished != ThreadCount )
			//    System.Threading.Thread.Sleep( 100 );

			int empty = 0;
			int read = 0;
			{
				//Console.WriteLine( "Beginning to read..." );
				System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
				foreach ( var thread in threads )
					thread.Start();

				sw.Start();
				do
				{
#if TEST_VERBOSE_MULTIWRITERQUEUE
					var node = queue.First;
#endif
					var box = queue.Read();
					if ( box != null )
					{
						empty = 0;
						//System.Threading.Interlocked.Decrement( ref count );
						KeyValuePair<int, Data> value = box.Value;
#if TEST_VERBOSE_MULTIWRITERQUEUE
						Console.WriteLine( "  < read " + node );
						Console.WriteLine( "\t" + queue.First );
						Console.WriteLine( "\t" + queue.Last );
#endif
						if ( results.ContainsKey( value.Key ) && results[value.Key] != value.Value - 1 )
						{
							if ( results[value.Key] < value.Value )
								Console.WriteLine( "Missing value from writer " + value.Key + ": previous=" + results[value.Key] + ", current=" + value.Value );
							else
								Console.WriteLine( "Duplicated value from writer " + value.Key + ": previous=" + results[value.Key] + ", current=" + value.Value );
						}
						else
							read++;

						results[value.Key] = value.Value;
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
				//Console.WriteLine( ">>> " + read + " items from " + ThreadCount + " threads received in " + sw.ElapsedMilliseconds + "ms (" + (int)( read / ( sw.ElapsedMilliseconds / 1000.0 ) ) + " messages/second)." );
				Report.Append( "" + read + " items from " + ThreadCount + " threads received in " + sw.ElapsedMilliseconds + "ms (" + (int)( read / ( sw.ElapsedMilliseconds / 1000.0 ) ) + " messages/second)." ).AppendLine();
				Console.WriteLine( ">>> " + read + " items(" + System.Runtime.InteropServices.Marshal.SizeOf( typeof( Data ) ) + "bytes each) from " + ThreadCount + " threads received in " + sw.ElapsedMilliseconds + "ms (" + (int)( read / ( sw.ElapsedMilliseconds / 1000.0 ) ) + " messages/second)." );
				Console.WriteLine();
			}
		}
#endif
#if TEST_SINGLEWRITERQUEUE
		static void TestSingleWriterQueue( int ItemCount )
		{
			GC.Collect();
			System.Threading.Thread.Sleep( 10 );
			GC.Collect();

			bool done = false;
			int ThreadCount = 1;
			Console.WriteLine( "Testing SingleWriterQueue..." );
#if !TEST_PERFORMANCE_SINGLEWRITERQUEUE
			Console.WriteLine( "\tConfiguration: " + BuildType + ", Threads=" + ThreadCount + ", Items=" + ItemCount + ", NO PERFORMANCE TEST" );
#else
			Console.WriteLine( "\tConfiguration: " + BuildType + ", Threads=" + ThreadCount + ", Items=" + ItemCount + ", ItemSize=" + System.Runtime.InteropServices.Marshal.SizeOf( typeof( Data ) ) + "bytes, performance test" );
#endif
			Console.WriteLine( "\tSystem: " + Environment.OSVersion + ", CPUs=" + Environment.ProcessorCount );
			zeroflag.Threading.LocklessQueueOld<Box<KeyValuePair<int, Data>>> queue = new LocklessQueueOld<Box<KeyValuePair<int, Data>>>();
			List<System.Threading.Thread> threads = new List<System.Threading.Thread>();
			int threadsFinished = 0;
			for ( int t = 0; t < ThreadCount; t++ )
				threads.Add(
				new System.Threading.Thread( () =>
				{
					//Console.WriteLine( System.Threading.Thread.CurrentThread.ManagedThreadId + "> Filling queue..." );
					System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
					sw.Start();
					Random rand = new Random();
					for ( int i = 0; i < ItemCount; i++ )
					{
#if TEST_VERBOSE_SINGLEWRITERQUEUE
						var item = new Box<KeyValuePair<int, int>>() { Value = new KeyValuePair<int, int>( System.Threading.Thread.CurrentThread.ManagedThreadId, i ) };
						queue.Push( item );
						Console.WriteLine( System.Threading.Thread.CurrentThread.ManagedThreadId + "> wrote " + item );
#else
						queue.Push( new Box<KeyValuePair<int, Data>>() { Value = new KeyValuePair<int, Data>( System.Threading.Thread.CurrentThread.ManagedThreadId, i ) } );
#endif
						//System.Threading.Interlocked.Increment( ref count );
#if !TEST_PERFORMANCE_SINGLEWRITERQUEUE
							System.Threading.Thread.Sleep( rand.Next( 10 ) );
#endif
					}
					sw.Stop();
					Console.WriteLine( System.Threading.Thread.CurrentThread.ManagedThreadId + "> Wrote " + ItemCount + " items(" + System.Runtime.InteropServices.Marshal.SizeOf( typeof( Data ) ) + "bytes each) in " + sw.ElapsedMilliseconds + "ms." );
					System.Threading.Interlocked.Increment( ref threadsFinished );
					while ( !done )
						queue.Update();
				} ) );
			Dictionary<int, int> results = new Dictionary<int, int>();

			//while ( threadsFinished != ThreadCount )
			//    System.Threading.Thread.Sleep( 100 );

			int empty = 0;
			int read = 0;
			{
				//Console.WriteLine( "Beginning to read..." );
				System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
				foreach ( var thread in threads )
					thread.Start();

				sw.Start();
				do
				{
					var box = queue.Pop();
					if ( box != null )
					{
						empty = 0;
						//System.Threading.Interlocked.Decrement( ref count );
						KeyValuePair<int, Data> value = box.Value;
#if TEST_VERBOSE_SINGLEWRITERQUEUE
						Console.WriteLine( "  < read " + node );
						Console.WriteLine( "\t" + queue.First );
						Console.WriteLine( "\t" + queue.Last );
#endif
						if ( results.ContainsKey( value.Key ) && results[value.Key] != value.Value - 1 )
						{
							if ( results[value.Key] < value.Value )
								Console.WriteLine( "Missing value from writer " + value.Key + ": previous=" + results[value.Key] + ", current=" + value.Value );
							else
								Console.WriteLine( "Duplicated value from writer " + value.Key + ": previous=" + results[value.Key] + ", current=" + value.Value );
						}
						else
							read++;

						results[value.Key] = value.Value;
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
									//Console.WriteLine( "\tFirst = " + ( queue.First ?? (object)"<null>" ).ToString() );
									//Console.WriteLine( "\tLast  = " + ( queue.Last ?? (object)"<null>" ).ToString() );
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
				done = true;
				Console.WriteLine( ">>> " + read + " items(" + System.Runtime.InteropServices.Marshal.SizeOf( typeof( Data ) ) + "bytes each) from " + ThreadCount + " threads received in " + sw.ElapsedMilliseconds + "ms (" + (int)( read / ( sw.ElapsedMilliseconds / 1000.0 ) ) + " messages/second)." );
				Console.WriteLine();
			}
		}
#endif
	}
}
