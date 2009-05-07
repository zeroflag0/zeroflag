using System;
namespace zeroflag.Forms.Reflected
{
	public interface IListView
	{
		//System.Windows.Forms.ListView Control { get; }

		/// <summary>
		/// Gets or sets the sort order for items in the control.
		/// </summary>
		System.Windows.Forms.SortOrder Sorting { get; set; }
		event Action<object> ItemAdded;
		event Action<object> ItemDeselected;
		event Action<object> ItemSelected;
		event Action<object> ItemRemoved;
		System.Collections.ICollection Items { get; set; }
		System.Collections.IList SelectedItems { get; }
		void Synchronize();
		//TypeDescription TypeDescription { get; set; }
	}
	[Serializable]
	public class ListViewControlBase : System.Windows.Forms.UserControl//, IListView
	{
		#region IListView Members

		//public abstract System.Windows.Forms.ListView Control
		//{
		//    get;
		//}

		//public abstract event Action<object> ItemAdded;

		//public abstract event Action<object> ItemDeselected;

		//public abstract event Action<object> ItemSelected;

		//public abstract event Action<object> ItemRemoved;

		//public abstract System.Collections.ICollection Items
		//{
		//    get;
		//    set;
		//}

		//public abstract System.Collections.ICollection SelectedItems
		//{
		//    get;
		//}

		//public abstract void Synchronize();

		//public abstract TypeDescription TypeDescription
		//{
		//    get;
		//    set;
		//}

		#endregion
	}
}
