using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	public class Calculator : Component
	{
		private Project _Project;
		/// <summary>
		/// the calculator's project
		/// </summary>
		public Project Project
		{
			get { return _Project; }
			set
			{
				if (_Project != value)
				{
					_Project = value;
					this.OnPropertyChanged("Project");
				}
			}
		}


		public void Run()
		{
			this.Log("Executing runs...");
			this.IsBusy = true;

			//foreach (var run in runsarray)
			//{
			//	this.Project.Results.Insert(0, run);
			//}

		}

		public Command RunSimulation
		{
			get
			{
				return new Command(p => this.Run());
			}
		}

	}
}
