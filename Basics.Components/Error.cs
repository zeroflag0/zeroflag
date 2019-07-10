using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Basics.Components
{
	public class Error : Content<Error>
	{
		public Error()
		{
		}

		public Error(string shortname, string description)
		{
			this.Name = shortname;
			this.Description = description;
		}

		
	}
}
