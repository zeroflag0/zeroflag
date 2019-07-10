using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	public interface IResult : ICalculatingContent, ICalculationResult
	{
	}

	public abstract class Result<TSelf> : CalculatingContent<TSelf, TSelf>, IResult
			where TSelf : Result<TSelf>, new()
	{

		protected override TSelf ProvideResult()
		{
			return (TSelf)this;
		}
	}
}
