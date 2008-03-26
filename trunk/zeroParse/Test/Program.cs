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
			ContextDebugForm debugForm = new ContextDebugForm();
			zeroParse.Parser parser = new CppParser();

			string[] files = null;
			//files = System.IO.Directory.GetFiles("source");
			//files = new string[] { "source/OgreAlignedAllocator.cpp" };
			files = new string[] { "test.txt" };
			foreach (string file in files)
			{
				Console.WriteLine("Processing file " + file + "...");
				Token document = null;
				ParserContext context = null;
				System.Threading.Thread thread = null;
				try
				{
					thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
						{
							parser.Parse(file, out context);
						}));
					thread.Start();
					thread.Join(5000);
					try
					{
						debugForm.ParserThread = thread;
						debugForm.Show(context);
					}
					catch (Exception exc)
					{
						Console.WriteLine(exc);
					}
				}
				catch (ParseFailedException exc)
				{
					context = exc.Context.RootContext;
					try
					{
						debugForm.Show(context);
						if (System.Windows.Forms.MessageBox.Show(exc.ToString(), "Show Context Trace?", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
							debugForm.Show(exc.ContextTrace);
					}
					catch (Exception exc2)
					{
						//Console.WriteLine(exc2);
					}
				}

				Console.WriteLine();
				if (document != null)
				{
					Console.WriteLine(document);
					try
					{
						if (context != document.Context.RootContext)
							debugForm.Show(document.Context.RootContext);
					}
					catch (Exception exc)
					{
						//Console.WriteLine(exc);
					}
				}
				//new RuleDebugForm(parser.Root).Show();
				//try
				//{
				try
				{
					while (thread.IsAlive)
					{
						System.Windows.Forms.Application.DoEvents();
						thread.Join(100);
					}
				}
				catch { }
				//}
				//catch (Exception exc)
				//{
				//    Console.WriteLine(exc);
				//}

				//foreach (Token token in document.Inner)
				//{
				//    Console.WriteLine(token.Name + ":=" + token.BlockValue);
				//}
				//Console.WriteLine((token ?? new zeroParse.Token() { Value = "<null>" }));
				Console.WriteLine("Finished file " + file + ".");
			}
			System.Windows.Forms.Application.Run(debugForm);
		}
	}
}
