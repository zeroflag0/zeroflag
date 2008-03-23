using System;
using System.Collections.Generic;
using System.Text;
using zeroParse;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			zeroParse.Parser parser = new CppParser();
			Token document = parser.Parse("test.c");
			if (document != null)
			{

				Console.WriteLine(document);

				//new RuleDebugForm(parser.Root).Show();
				try
				{
					System.Windows.Forms.Application.Run(new ContextDebugForm(document.Context));
				}
				catch (Exception exc)
				{
					Console.WriteLine(exc);
				}
					 
				//foreach (Token token in document.Inner)
				//{
				//    Console.WriteLine(token.Name + ":=" + token.BlockValue);
				//}
			}
			//Console.WriteLine((token ?? new zeroParse.Token() { Value = "<null>" }));
		}
	}
}
