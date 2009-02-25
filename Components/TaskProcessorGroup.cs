using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace zeroflag.Components
{
	public partial class TaskProcessorGroup : Component
	{
		public TaskProcessorGroup()
		{
			InitializeComponent();
		}
#if !SILVERLIGHT
		public TaskProcessor( System.ComponentModel.IContainer container )
		{
			container.Add( this );

			InitializeComponent();
		}
#endif
	}
}
