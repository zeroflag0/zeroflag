using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Serialization
{
	class Benchmark
	{
		static Benchmark _Instance = new Benchmark();

		internal static Benchmark Instance
		{
			get { return Benchmark._Instance; }
		}

		protected Benchmark()
		{
			this.Clock.Start();
		}

		System.Diagnostics.Stopwatch _Clock = new System.Diagnostics.Stopwatch();

		public System.Diagnostics.Stopwatch Clock
		{
			get { return _Clock; }
		}
		[System.Diagnostics.Conditional("PROFILE")]
		public void Trace(string message)
		{
			//Console.WriteLine(new StringBuilder().Append(this.Clock.ElapsedMilliseconds.ToString("0000")).Append("ms ").Append(message).ToString());
			System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
			
			System.Diagnostics.StackFrame frame = trace.GetFrame(1);
			//for (int i = 1; null != (frame = trace.GetFrame(i)); i++)
			//    if (frame.GetMethod().DeclaringType != this.GetType())
			//        break;

			this.Trace(message, frame);
		}
		[System.Diagnostics.Conditional("PROFILE")]
		public void Trace(params object[] parameters)
		{
			this.Trace("", parameters);
		}
		[System.Diagnostics.Conditional("PROFILE")]
		public void Trace(string message, params object[] parameters)
		{
			var b = new StringBuilder().Append(this.Clock.ElapsedMilliseconds.ToString("0000")).Append("ms ").Append(message);
			b.Append("(");
			foreach (object param in parameters)
			{
				b.Append(param).Append(", ");
			}
			b.Append(")");
			Console.WriteLine(b.ToString());
		}

		[System.Diagnostics.Conditional("PROFILE")]
		public void Trace()
		{
			this.Trace("trace", new System.Diagnostics.StackTrace().GetFrame(1));
		}

	}
}
