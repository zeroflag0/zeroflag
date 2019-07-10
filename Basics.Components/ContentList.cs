using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;


namespace Basics.Components
{
	/// <summary>
	/// model content list
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
	public class ContentList<TItem> : ContentList<TItem, ContentList<TItem>>
		where TItem : Content<TItem>, new()
	{
		public ContentList()
		{
		}

		public ContentList(string name)
			: base(name)
		{
		}
	}

	/// <summary>
	/// model content list
	/// </summary>
	public class ContentList<TItem, TSelf> : SavableList<TItem>
		where TItem : Content<TItem>, new()
		where TSelf : ContentList<TItem, TSelf>, new()
	{
		protected override void OnLockChanged(bool value)
		{
			base.OnLockChanged(value);

			foreach (TItem item in this)
			{
				item.IsLocked = value;
			}
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		public TSelf Duplicate()
		{
			TSelf clone = new TSelf();
			this.FillClone(clone);
			clone.ClearStorageIds();
			return clone;
		}

		protected virtual void FillClone(TSelf clone)
		{
			clone.Clear();
			foreach (TItem item in this)
			{
				TItem citem = item.Duplicate();
				clone.Add(citem);
			}
		}

		public ContentList()
		{
		}

		public ContentList(string name)
			: base(name)
		{
		}
	}
}
