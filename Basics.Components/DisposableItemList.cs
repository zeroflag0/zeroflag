using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	public class DisposableItemList<T> : ItemList<T>
		where T : class, IDisposable//, new()
	{
		//public Command AddCommand
		//{
		//	get
		//	{
		//		return new Command(p =>
		//		{
		//			this.Add(new T());
		//		});
		//	}
		//}

		public override void Dispose()
		{
			lock (this)
			{
				var parameters = this.Items.ToArray();
				this.Clear();
				foreach (T item in parameters)
				{
					item.Dispose();
				}
			}
			base.Dispose();
		}

		public DisposableItemList(string name)
			: base(name)
		{
		}

		public DisposableItemList()
		{
		}
	}
}
