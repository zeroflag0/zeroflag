using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Zml
{
	public abstract class Factory<Product> : zeroflag.Zml.IFactory
	{
		public Factory()
		{
		}

		public Type ProductType
		{
			get { return typeof(Product); }
		}

		public abstract Product Create();

		#region IFactory Members

		object IFactory.Create()
		{
			return this.Create();
		}

		#endregion
	}
}
