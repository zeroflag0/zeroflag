using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	public class JobContext<TInput> : JobContext
		where TInput : class
	{

		private TInput _Input;

		/// <summary>
		/// input object
		/// </summary>
		[IgnoreErrors]
		public virtual TInput Input
		{
			get { return _Input; }
			set
			{
				if (_Input != value)
				{
					_Input = value;
					this.OnPropertyChanged("Input");
				}
			}
		}

	}
}
