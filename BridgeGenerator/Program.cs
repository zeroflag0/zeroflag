using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace zeroflag.BridgeGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				//MethodInfo method = (MethodInfo)typeof(List<>).GetMember("ConvertAll")[0];
				//ConstructorInfo constructor = typeof(List<>).GetConstructor(new Type[] { });
				//constructor.Equals(method);		//<-- works fine...
				//method.Equals(constructor);		//<-- crashes, NullReferenceException

				Implementor imp = new Implementor();

				// the bridge should implement these interfaces'/classes' public properties and methods...
				imp.Interfaces.Add(typeof(IList<>));
				imp.Interfaces.Add(typeof(List<>));

				//imp.IgnoreInterfaces.Add(typeof(List<>));
				//imp.IgnoreInterfaceMembers = true;

				// the actual implementation/bridge-target should be stored in...
				imp.Property = "Items";
				// and should be of type...
				imp.Implementation = typeof(List<>);
				//imp.BaseType = typeof(System.Windows.Forms.Control);
				imp.ImplementOverrides = false;


				// the name of the generated class is...
				imp.ClassName = "List";

				// should we bridge constructors?
				imp.BridgeConstructors = true;

				string result = imp.Generate();
				Console.WriteLine(result);
				System.IO.File.WriteAllText(imp.ClassName + ".cs", result);
				System.IO.File.WriteAllText("result.cs", result);

				//if (System.IO.File.Exists(imp.ClassName + ".meta.cs"))
				//{
				//    Metadata meta = new Metadata();
				//    meta.SetMembers(imp.Members);

				//    meta.Run(System.IO.File.ReadAllText(imp.ClassName + ".meta.cs"));

				//    new zeroflag.Serialization.XmlSerializer(imp.ClassName + ".meta.xml").Serialize(meta);
				//}
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
			}
		}
	}
}
