using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	/// <summary>
	/// Baseclass for user calculations. Provides convenience functions, helpers and general data API.
	/// </summary>
	//[Serializable]
	public abstract partial class JobBase : Base
	{
		public JobBase()
		{
		}

		public override void Dispose()
		{
			base.Dispose();
		}

		/// <summary>
		/// short description of the calculation...
		/// </summary>
		public abstract string Name { get; }
		/// <summary>
		/// detailed description of the calculation...
		/// </summary>
		public abstract string Description { get; }

		private JobContext _Context;

		/// <summary>
		/// the calculation's context
		/// </summary>
		[IgnoreErrors]
		public JobContext Context
		{
			get { return _Context; }
			set
			{
				if (_Context != value)
				{
					_Context = value;
					this.OnPropertyChanged("Context");
				}
			}
		}

		/// <summary>
		/// 1. [optional] perform basic initialization (without data)
		/// </summary>
		public virtual void InitializeBasics()
		{
		}

		/// <summary>
		/// 2. create all required fields, parameters and summaries
		/// </summary>
		public abstract void CreateFields();

		/// <summary>
		/// 3. initialize fields, read parameters.
		/// </summary>
		public abstract void Initialize();
		/// <summary>
		/// 4. [optional] prepare calculation at timestep 0.
		/// </summary>
		public virtual void Prepare()
		{
		}

		/// <summary>
		/// 5. calculate the current timestep.
		/// </summary>
		/// <param name="step"></param>
		public abstract void Calculate();

		/// <summary>
		/// 5.a) performed after each calculation. decides whether to continue.
		/// </summary>
		/// <param name="step"></param>
		/// <returns></returns>
		public abstract bool CheckContinue();

		/// <summary>
		/// 6. additional steps performed after all timesteps (Calculate) are done. e.g. summaries.
		/// </summary>
		public abstract void Finish();

		/// <summary>
		/// validate parameters and runtime
		/// </summary>
		/// <returns></returns>
		public bool Validate()
		{
			lock (this)
			{
				bool result = true;

				return result;
			}
		}
		/// <summary>
		/// validates runtime
		/// </summary>
		/// <returns></returns>
		public abstract bool ValidateRun();
	}
}
