using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Reset();
			sw.Start();

			zeroflag.Zml.TypeFinder finder = zeroflag.Zml.TypeFinder.Instance;

			sw.Stop();

			Console.WriteLine("Finder creation took " + sw.Elapsed);

			sw.Reset();

			FindTest test = key =>
				{
					System.Diagnostics.Stopwatch sw2 = new System.Diagnostics.Stopwatch();
					sw2.Start();
					sw2.Reset();
					sw2.Start();
					Type t = null;
					List<Type> results = null;
					int i = 1;
					//for (; i < 1000000 && sw2.ElapsedMilliseconds < 10000; i++)
					//t = finder[key];
					results = finder.SearchAll(key);
					sw2.Stop();
					if (results != null && results.Count > 0)
						t = results[0];
					else
						t = null;


					Console.WriteLine("Search took " + ((double)sw2.ElapsedMilliseconds / (double)1).ToString() + "ms (" + i + "x) and returned " + (t ?? (object)"<null>") + " for " + key);
					Console.WriteLine("More:");
					foreach (Type result in results)
						Console.WriteLine("\t" + result);
				};
			//finder.SearchAll("document");
			//return;
			string[] testKeys = new string[]
			{
				"list", "string", "Type", "TypeFinder", "stream", "document"
			};

			foreach (string key in testKeys)
				test(key);
		}

		delegate void FindTest(string key);
	}
}
