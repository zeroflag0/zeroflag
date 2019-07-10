using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	public interface IJobManager
	{
		JobContext CreateContext();
		JobContext ActiveContext { get; set; }
	}

	/// <summary>
	/// manages several calculation contexts
	/// </summary>
	public class JobManager<TContext> : Base, IJobManager
		where TContext : JobContext, new()
	{
		public TContext CreateContext()
		{
			TContext context = new TContext();
			context.Manager = this;

			this.Contexts.Add(context);
			this.ActiveContext = context;
			return context;
		}

		private TContext _ActiveContext;
		/// <summary>
		/// the currently active or last active calculation context.
		/// </summary>
		public TContext ActiveContext
		{
			get { return _ActiveContext; }
			set
			{
				if (_ActiveContext != value)
				{
					_ActiveContext = value;
					this.OnPropertyChanged("ActiveContext");
				}
			}
		}

		private DisposableItemList<TContext> _Contexts = new DisposableItemList<TContext>();
		/// <summary>
		/// the last contexts that have been calculated.
		/// </summary>
		public DisposableItemList<TContext> Contexts
		{
			get { return _Contexts; }
		}

		public override void Dispose()
		{
			base.Dispose();

			var contexts = this.Contexts.ToArray();
			this.Contexts.Clear();
			this._Contexts = null;
			foreach (TContext context in contexts)
			{
				context.Dispose();
			}

			this.ActiveContext = null;
		}

		#region ICalculationManager Members

		JobContext IJobManager.CreateContext()
		{
			return this.CreateContext();
		}

		JobContext IJobManager.ActiveContext
		{
			get
			{
				return this.ActiveContext;
			}
			set
			{
				this.ActiveContext = (TContext)value;
			}
		}

		#endregion
	}
	///// <summary>
	///// manages several calculation contexts
	///// </summary>
	//public abstract class CalculationManager : Base, ICalculationManager
	//{

	//	#region ICalculationManager Members

	//	public JobContext CreateContext()
	//	{
	//		throw new NotImplementedException();
	//	}

	//	public JobContext ActiveContext
	//	{
	//		get
	//		{
	//			throw new NotImplementedException();
	//		}
	//		set
	//		{
	//			throw new NotImplementedException();
	//		}
	//	}

	//	#endregion
	//}
}
