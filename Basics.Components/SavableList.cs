using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;


namespace Basics.Components
{
	public class SavableList<TItem>
		: SavableGenericList<TItem>
		where TItem : class, ISavable, new()
	{
		protected static TItem LoadItem(DataSet data, int id, Type type)
		{
			return Savable.Load<TItem>(data, id);
		}

		public SavableList()
			: base(LoadItem)
		{
		}

		public SavableList(string name)
			: base(LoadItem, name)
		{
		}
	}

	public class SavableGenericList<TItem>
		: DisposableItemList<TItem>
		where TItem : class, ISavable
	{
		public SavableGenericList(LoadHandler load)
			: this(load, null)
		{
		}

		public SavableGenericList(LoadHandler load, string name)
			: base(name)
		{
			this.LoadAction = load;
		}

		public delegate TItem LoadHandler(DataSet data, int id, Type type);
		public LoadHandler LoadAction { get; set; }

		protected string StorageItemName
		{
			get { return this.StorageName + ".Items"; }
		}

		public override void ClearStorageIds()
		{
			base.ClearStorageIds();
			foreach (TItem item in this)
			{
				item.ClearStorageIds();
			}
		}

		protected override DataTable CreateStorageTable(DataSet data)
		{
			DataTable table = this.GetTable(data, this.StorageItemName);

			DataColumnCollection columns = table.Columns;
			if (!columns.Contains("listid"))
			{
				DataColumn col = columns.Add("listid", typeof(int));
				col.AllowDBNull = false;
			}
			if (!columns.Contains("itemid"))
			{
				DataColumn col = columns.Add("itemid", typeof(int));
				col.AllowDBNull = false;
			}
			if (!columns.Contains("type"))
			{
				DataColumn col = columns.Add("type", typeof(Type));
				col.AllowDBNull = false;
			}
			table.PrimaryKey = new DataColumn[]
			{ 
				table.Columns["listid"], 
				table.Columns["itemid"],
			};
			return base.CreateStorageTable(data);
		}

		protected override void SavableFields(SavableFields fields)
		{
			//NOTE: must not call base.SavableFields(fields)!!!
		}

		protected override void CustomSave(DataSet data, DataTable table)
		{
			base.CustomSave(data, table);

			DataTable itemtable = this.GetTable(data, this.StorageItemName);
			foreach (TItem item in this)
			{
				// have the item save itself and get it's id...
				int i = item.Save(data);

				DataRow row;
				{
					// produce a new row and store it...
					row = itemtable.NewRow();
					// set the list id and the item id...
					row["listid"] = this.StorageId;
					row["itemid"] = i;
					row["type"] = item.GetType();
					itemtable.Rows.Add(row);
				}
			}
		}

		protected override void CustomLoad(DataSet data, DataTable table)
		{
			using (this.Freeze())
			{
				base.CustomLoad(data, table);

				// cache any existing rows...
				Dictionary<int, TItem> existing = new Dictionary<int, TItem>();
				foreach (TItem item in this.ToArray())
				{
					if (item.StorageId != null)
						existing.Add(item.StorageId.Value, item);
					else
						// remove existing rows that were not stored because we cannot link them to the database...
						this.Remove(item);
				}

				DataTable itemtable = this.GetTable(data, this.StorageItemName);

				// fetch the rows that belong to this list...
				DataRow[] rows =
					(from row in itemtable.AsEnumerable()
					 where SavableField<int>.Parse(row["listid"]) == this.StorageId
					 select row).ToArray();

				HashSet<int> found = new HashSet<int>();
				foreach (DataRow row in rows)
				{
					// parse the item id...
					int i = SavableField<int>.Parse(row["itemid"]);
					found.Add(i);

					Type type = SavableField<Type>.Parse(row["type"]);

					TItem item;
					if (existing.ContainsKey(i))
					{
						// if it is already linked keep it...
						item = existing[i];
					}
					else
					{
						// have the item load itself...
						item = this.LoadAction(data, i, type);
						this.Add(item);
					}
				}

				// compare the list of old an processed items...
				var notfound = (from ex in existing.Keys
								where !found.Contains(ex)
								select ex).ToArray();
				foreach (int i in notfound)
				{
					// remove any item that existed before and is no longer part of this list...
					TItem item = existing[i];
					this.Remove(item);
				}
			}
		}

	}
}
