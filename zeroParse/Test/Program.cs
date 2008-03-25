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
			Token document = null;
			ParserContext context = null;
			try
			{
				document = parser.Parse("test.cpp", out context);
				try
				{
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
					Console.WriteLine(exc2);
				}
			}
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
					Console.WriteLine(exc);
				}
			}
			//new RuleDebugForm(parser.Root).Show();
			try
			{
				System.Windows.Forms.Application.Run(debugForm);
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
			}

			//foreach (Token token in document.Inner)
			//{
			//    Console.WriteLine(token.Name + ":=" + token.BlockValue);
			//}
			//Console.WriteLine((token ?? new zeroParse.Token() { Value = "<null>" }));
		}
	}
}
