using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Basics
{
	public class IsNullConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (value == null);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException("IsNullConverter can only be used OneWay.");
		}

	}
	public class HexConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is int) return ((int)value).ToString("x");
			if (value is long) return ((long)value).ToString("x");
			if (value is byte) return ((byte)value).ToString("x");
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			try
			{
				return long.Parse(value + "", NumberStyles.AllowHexSpecifier);
			}
			catch (Exception)
			{
				return value;
			}
		}

	}
	public class LessThanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is System.Windows.Controls.Primitives.DataGridColumnHeader)
				value = (value as System.Windows.Controls.Primitives.DataGridColumnHeader).ActualWidth;
			if (value == null)
				return value;

			double limit;
			if (parameter is double)
				limit = (double)parameter;
			else if (parameter is int)
				limit = (double)(int)parameter;
			else limit = double.Parse(parameter.ToString());

			if (value is double)
			{
				if (double.IsNaN((double)value))
					return value;
				return ((double)value) < (double)limit;
			}
			else if (value is int)
			{
				return ((int)value) < limit;
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException("IsNullConverter can only be used OneWay.");
		}

	}
	public class DebugConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

	}

	public class IsDebugConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
#if DEBUG
			return true;
#else
			return false;
#endif
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}

	public class DataGridTableConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			System.Windows.Controls.DataGridCell cell = value as System.Windows.Controls.DataGridCell;
			var first = value as System.Windows.FrameworkElement;
			var control = value as System.Windows.FrameworkElement;
			while (cell == null)
			{
				if (control == null)
					return null;
				cell = control.Parent as System.Windows.Controls.DataGridCell;
				control = control.Parent as System.Windows.FrameworkElement;
			}

			System.Windows.Controls.DataGridTextColumn column = cell.Column as System.Windows.Controls.DataGridTextColumn;
			Binding binding;
			binding = (Binding)column.Binding;

			if (binding == null)
				return null;

			first.SetBinding(System.Windows.Controls.TextBlock.TextProperty, binding);

			return ((System.Windows.Controls.TextBlock)first).Text;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

	}

}