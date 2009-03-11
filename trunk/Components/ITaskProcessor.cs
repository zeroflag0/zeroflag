using System;
namespace zeroflag.Components
{
	public interface ITaskProcessor
	{
		void Add<T1, T2>( Action<T1, T2> task, T1 p1, T2 p2 );
		void Add<T1, T2, T3>( Action<T1, T2, T3> task, T1 p1, T2 p2, T3 p3 );
		void Add( params Action[] tasks );
		void Add<T1>( Action<T1> task, T1 p1 );
		void Add<T1, T2, T3, T4>( Action<T1, T2, T3, T4> task, T1 p1, T2 p2, T3 p3, T4 p4 );
		void Add<T1, T2, T3>( DateTime time, Action<T1, T2, T3> task, T1 p1, T2 p2, T3 p3 );
		void Add<T1, T2, T3, T4>( DateTime time, Action<T1, T2, T3, T4> task, T1 p1, T2 p2, T3 p3, T4 p4 );
		void Add<T1>( DateTime time, Action<T1> task, T1 p1 );
		void Add<T1, T2>( DateTime time, Action<T1, T2> task, T1 p1, T2 p2 );
		void Add( DateTime time, params Action[] tasks );
		void Add( Task task );
		bool Cancel { get; set; }
		void Clear();
		int Count { get; }
		string Current { get; }
		Task? CurrentAction { get; }
		bool Disposing { get; set; }
		event TaskProcessor.DisposingChangedHandler DisposingChanged;
		event TaskProcessor.ErrorHandlingHandler ErrorHandling;
		void Finish();
		bool IsRunning { get; }
		string Name { get; set; }
		System.Threading.ThreadPriority ThreadPriority { get; set; }
	}
}
