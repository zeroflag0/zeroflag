using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace zeroflag.Forms.Reflected
{
	public partial class ListViewItem<List, T> : System.Windows.Forms.ListViewItem
		where List : ListView<T>
	{
		#region Owner

		private List _Owner = default(List);

		public List Owner
		{
			get { return _Owner; }
			set
			{
				if (_Owner != value)
				{
					_Owner = value;
					if (_Owner != null)
						_Owner.ListViewControl.Items.Add(this);
				}
			}
		}
		#endregion Owner

		#region Value

		private T _Value = default(T);

		public T Value
		{
			get { return _Value; }
			set
			{
				if (object.ReferenceEquals(_Value, null) ||
					!object.ReferenceEquals(null, value) && !value.Equals(_Value))
				{
					_Value = value;
				}
			}
		}
		#endregion Value

		public ListViewItem(List list, T value)
		{
			this.Owner = list;
			this.Value = value;
		}

		public void Synchronize()
		{
			TypeDescription desc = this.Owner.ItemDescription;
		}
	}
}
