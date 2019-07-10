using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Basics.Components;

namespace Basics.Components
{
	/// <summary>
	/// provides a context, parameters and results for a single job(-group)
	/// </summary>
	public class JobContext : ErrorInfo, ICalculationResult
	{
		public JobContext()
		{
		}

		public override void Dispose()
		{
			base.Dispose();


			if (this.Jobs != null)
			{
				foreach (var calc in this.Jobs)
				{
					calc.Dispose();
				}
				this.Jobs.Clear();
			}
			this._Jobs = null;

			this._ResultColumns = null;
			this._Results = null;
		}

		private DataTable _Results = new DataTable("results");

		private Parameters _Summaries = new Parameters();

		Parameters _Parameters = new Parameters();

		Dictionary<string, DataColumn> _ResultColumns = new Dictionary<string, DataColumn>();

		Fields _Fields = new Fields();

		void FieldsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems != null)
			{
				foreach (Field item in e.NewItems)
				{
					//this.Log("Added ValueDescription for '" + item.Name + "'");
					//this.ValueDescriptions.Add(item.Name, item);
					this.OnFieldAdded(item);
				}
			}
			//else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems != null)
			//{
			//	foreach (Field item in e.OldItems)
			//	{
			//		this.ValueDescriptions.Remove(item.Name);
			//	}
			//}
		}

		protected virtual void OnFieldAdded(Field item)
		{
		}

		private IJobManager _Manager;
		/// <summary>
		/// the manager for this context
		/// </summary>
		[IgnoreErrors]
		public IJobManager Manager
		{
			get
			{
				return _Manager;
			}
			set
			{
				if (_Manager != value)
				{
					_Manager = value;
					this.OnPropertyChanged("Manager");
				}
			}
		}

		List<JobBase> _Jobs;
		/// <summary>
		/// the calculations to be used
		/// </summary>
		public List<JobBase> Jobs
		{
			get { return _Jobs; }
			set { _Jobs = value; }
		}

		public void Execute()
		{
			if (this.Jobs == null || this.Jobs.Count == 0)
				throw new InvalidOperationException("The calculation was not initialized. (Did you call Initialize() first?)");

			IEnumerable<JobBase> calculations = this.Jobs;
			foreach (JobBase calc in calculations)
				calc.Initialize();

			//this.ExecuteMainLoop();

			foreach (JobBase calc in calculations)
				calc.Finish();

		}


		public bool Validate()
		{
			// can't be valid if we don't have ..
			if (this.Jobs == null || this.Jobs.Count == 0)
				return false;

			// if any calculation fails to validate, everything is invalid...
			if (this.Jobs.Any(calc => !calc.Validate()))
				return false;

			// all good.
			return true;
		}

		public void Initialize()
		{
			if (this.Jobs == null)// || this.Count == 0)
				throw new InvalidOperationException("The calculation was not initialized. (Did you call Initialize() first?)");
			Stack<JobBase> freezeJobs = new Stack<JobBase>(this.Jobs);

			// freeze ui updates...
			this.FreezeAll(freezeJobs, () =>
				{
					// perform basic initialization (without data)...
					foreach (JobBase job in this.Jobs)
						job.InitializeBasics();

					// have the calculations create their desired fields...
					foreach (JobBase job in this.Jobs)
						job.CreateFields();
				});
		}

		/// <summary>
		/// Freezes ui-updates for all parameters and summaries on the objects passed in <paramref name="calcs"/> for the for the duration of <paramref name="action"/>
		/// </summary>
		/// <param name="calcs">the objects to be frozen</param>
		/// <param name="action">the action to be performed</param>
		void FreezeAll(Stack<JobBase> calcs, Action action)
		{
			if (calcs.Count == 0)
				return;
			JobBase calc = calcs.Pop();
			{
				if (calcs.Count > 0)
					this.FreezeAll(calcs, action);
				else
					action();
			}
		}

		protected override void SavableFields(SavableFields fields)
		{
		}
	}
}
