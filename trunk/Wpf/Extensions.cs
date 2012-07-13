using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

public static class Extensions
{
	public static void BeginInvoke(this DependencyObject self, Action a)
	{
		self.Dispatcher.BeginInvoke(a);
	}
	public static void BeginInvoke(this DispatcherObject self, Action a)
	{
		self.Dispatcher.BeginInvoke(a);
	}
}
