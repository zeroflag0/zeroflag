using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Input;

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
