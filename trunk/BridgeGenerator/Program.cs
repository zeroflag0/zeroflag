using System;
using System.Collections.Generic;
using System.Text;

namespace BridgeInterface
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Implementor imp = new Implementor();

				// the bridge should implement these interfaces'/classes' public properties and methods...
				imp.Interfaces.Add(typeof(IList<>));
				
				// the actual implementation/bridge-target should be stored in...
				imp.Property = "Items";
				// and should be of type...
				imp.Implementation = typeof(List<>);


				// the name of the generated class is...
				imp.ClassName = "IOBank";

				// should we bridge constructors?
				imp.BridgeConstructors = false;

				string result = imp.Generate();
				Console.WriteLine(result);
				System.IO.File.WriteAllText(imp.ClassName + ".cs", result);
				System.IO.File.WriteAllText("result.cs", result);
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
			}
		}
	}
}
