using System;
namespace zeroflag.Forms.Reflected
{
	interface IListView
	{
		System.Windows.Forms.ListView Control { get; }
		event Action<object> ItemAdded;
		event Action<object> ItemDeselected;
		event Action<object> ItemSelected;
		event Action<object> ItemRemoved;
		System.Collections.ICollection Items { get; set; }
		System.Collections.ICollection SelectedItems { get; }
		void Synchronize();
		TypeDescription TypeDescription { get; set; }
	}

	public abstract class ListViewControlBase : System.Windows.Forms.UserControl, IListView
	{
	}
}
