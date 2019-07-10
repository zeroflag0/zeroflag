using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basics;

namespace Basics.Components
{
	/// <summary>
	/// model component
	/// </summary>
	public class Component : Base
	{

		private bool _IsBusy;
		/// <summary>
		/// is busy
		/// </summary>
		public virtual bool IsBusy
		{
			get { return _IsBusy; }
			set
			{
				if (_IsBusy != value)
				{
					_IsBusy = value;
					this.OnPropertyChanged("IsBusy");
				}
			}
		}

		public void Busy(Action action, params Action[] actions)
		{
			this.BusyWait(null, action, actions);
		}

		public void BusyWait(Action finish, Action action, params Action[] actions)
		{
			if (action == null)
				return;

			if (finish == null)
				finish = () => this.IsBusy = false;
			else
			{
				Action innerfinish = finish;
				finish = () =>
				{
					try
					{
						innerfinish();
					}
					finally
					{
						this.IsBusy = false;
					}
				};
			}

			TaskBase task;
			if (actions == null || actions.Length == 0)
				task = new Task(action, finish);
			else
				task = new TaskGroup(finish, action, actions);

			this.IsBusy = true;
			task.RunAsync();
		}
	}
}
