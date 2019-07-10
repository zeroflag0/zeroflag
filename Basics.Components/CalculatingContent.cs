using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Basics.Components;

namespace Basics.Components
{
	public interface ICalculatingContent : IContent
	{
		void Execute();
		DateTime? CalculationTime { get; }
		void Initialize();
		/// <summary>
		/// the latest result calculated
		/// </summary>
		ICalculationResult CurrentResult { get; }
	}

	public abstract class CalculatingContent<TResult, TSelf> : Content<TSelf>, ICalculationResult, ICalculatingContent
		where TSelf : CalculatingContent<TResult, TSelf>, new()
		where TResult : class, ICalculationResult
	{
		public void Execute()
		{
			lock (this)
			{
				this.Log("Executing...", false);

				TResult result = this.ProvideResult();
				lock (result)
				{
					this.DoExecute(result);
				}
				this.CurrentResult = result;

				this.CalculationTime = DateTime.Now;
				this.Log("Executing... done.", true);
			}
		}


		private TResult _CurrentResult;

		/// <summary>
		/// the latest result calculated
		/// </summary>
		public TResult CurrentResult
		{
			get { return _CurrentResult; }
			set
			{
				if (_CurrentResult != value)
				{
					_CurrentResult = value;
					this.OnPropertyChanged("CurrentResult");
				}
			}
		}

		ICalculationResult ICalculatingContent.CurrentResult
		{
			get { return this.CurrentResult; }
		}

		protected abstract TResult ProvideResult();

		protected abstract void DoExecute(TResult result);

		private DateTime? _CalculationTime;
		/// <summary>
		/// when the run was calculated
		/// </summary>
		public DateTime? CalculationTime
		{
			get { return _CalculationTime; }
			set
			{
				if (_CalculationTime != value)
				{
					_CalculationTime = value;
					this.OnPropertyChanged("CalculationTime");
				}
			}
		}

		public abstract void Initialize();
	}
}
