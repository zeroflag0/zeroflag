using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

public static class Extensions
{
	public static void BeginInvoke(this DependencyObject self, Action a)
	{
		self.Dispatcher.BeginInvoke(a);
	}
}
