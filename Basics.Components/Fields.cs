using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	public class Fields : ValueList<Field, Fields>
	{
		static int IdCounter;
		public int Id { get; private set; }

		public Fields()
		{
			this.Id = System.Threading.Interlocked.Increment(ref IdCounter);
#if VERBOSE
			this.IsVerbose = true;
			this.Verbose("new Fields()" + Environment.NewLine + new System.Diagnostics.StackTrace().ToString());
#endif
		}

		public override string ToString()
		{
			return base.ToString() + "(" + this.Id + ")";
		}
	}
}
