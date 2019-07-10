using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TraceMerge
{
	public enum ProcessMode
	{
		Merge,
		Append,
		AppendRaw,
		//AppendInFile,
		Trim
	}
	[Serializable]
	public enum DirProcessModes
	{
		Merge,
		Trim,
	}
}
