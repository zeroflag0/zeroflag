using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	public interface IValue
	{
		string Name { get; set; }
		decimal? Value { get; set; }
		string Description { get; set; }
		string Format { get; set; }
	}
}
