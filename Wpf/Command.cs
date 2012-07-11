using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace zeroflag.wpf
{
	public class Command : ICommand
	{
		private Action<object> _Execute;
		private Func<object, bool> _CanExecute;

		public Command(Action<object> execute)
			: this(execute, null, null)
		{
		}

		public Command(Action<object> execute, Func<object, bool> canExecute)
			: this(execute, canExecute, null)
		{
		}

		public Command(Action<object> execute, Func<object, bool> canExecute, EventHandler canExecuteChanged)
		{
			_Execute = execute;
			_CanExecute = canExecute;
			if (canExecuteChanged != null)
				CanExecuteChanged += canExecuteChanged;
		}

		public bool CanExecute(object parameter)
		{
			if (_CanExecute == null)
			{
				return true;
			}
			return _CanExecute(parameter);
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			if (_Execute != null)
			{
				_Execute(parameter);
			}
		}
	}
}