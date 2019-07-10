using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

public static class UiExtensions
{
	public static TItem Find<TItem>(this FrameworkElement ctrl, Func<FrameworkElement, TItem> check)
		where TItem : class
	{
		if (ctrl == null)
			return null;
		TItem result;
		result = check(ctrl);
		if (result != null)
		{
			return result;
		}
		if (ctrl is ContentControl)
		{
			ContentControl container = (ContentControl)ctrl;
			if (container.Content == null)
				return null;
			if (!(container.Content is FrameworkElement))
				return null;
			FrameworkElement content = container.Content as FrameworkElement;
			return content.Find<TItem>(check);
		}
		if (ctrl is Panel)
		{
			Panel container = (Panel)ctrl;
			foreach (UIElement element in container.Children)
			{
				if (element == null)
					continue;
				TItem item = (element as FrameworkElement).Find<TItem>(check);
				if (item == null)
					continue;
				return item;
			}
		}

		return null;
	}

}
