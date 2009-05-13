using System;

namespace System.ComponentModel
{
	public class Component : IDisposable
	{
		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose( true );
		}

		#endregion

		protected virtual void Dispose( bool disposing )
		{
		}
	}
}
