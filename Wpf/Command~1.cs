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
	public class Command<T> : ICommand
	{
		private Action<T> _Execute;
		private Func<T, bool> _CanExecute;

		public Command(Action<T> execute)
			: this(execute, null, null)
		{
		}

		public Command(Action<T> execute, Func<T, bool> canExecute)
			: this(execute, canExecute, null)
		{
		}

		public Command(Action<T> execute, Func<T, bool> canExecute, EventHandler canExecuteChanged)
		{
			_Execute = execute;
			_CanExecute = canExecute;
			if (canExecuteChanged != null)
				CanExecuteChanged += canExecuteChanged;
		}

		public bool CanExecute(T parameter)
		{
			if (_CanExecute == null)
			{
				return true;
			}
			return _CanExecute(parameter);
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(T parameter)
		{
			if (_Execute != null)
			{
				_Execute(parameter);
			}
		}

		public bool CanExecute(object parameter)
		{
			return this.CanExecute((T)parameter);
		}

		public void Execute(object parameter)
		{
			this.Execute((T)parameter);
		}
	}
}