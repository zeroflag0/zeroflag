using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Basics.Components
{
	public interface ISavable : IDisposable
	{
		int Save(DataSet data);
		object Load(DataSet data, int id);
		void ClearStorageIds();
		int? StorageId { get; }
	}


	public abstract class Savable : Component, ISavable
	{
		/// <summary>
		/// the name this class has in the storage dataset...
		/// </summary>
		protected internal virtual string StorageName
		{
			get { return this.GetType().Namespace + "-" + this.GetType().Name; }
		}

		/// <summary>
		/// resets all storage ids for this item and it's children...
		/// </summary>
		public virtual void ClearStorageIds()
		{
			this.StorageId = null;
		}

		const string StorageIdField = "savable_id";
		int? _StorageId;
		/// <summary>
		/// the id representing this object...
		/// </summary>
		public int? StorageId
		{
			get { return _StorageId; }
			set { _StorageId = value; }
		}

		SavableFields _StorageFields = null;
		/// <summary>
		/// fields that will be saved and loaded...
		/// </summary>
		protected SavableFields StorageFields
		{
			get { return _StorageFields; }
			set { _StorageFields = value; }
		}

		/// <summary>
		/// creates the columns that will store the data for the class...
		/// </summary>
		/// <param name="table"></param>
		protected virtual DataTable CreateStorageTable(DataSet data)
		{
			this.CreateSavableFields();

			DataTable table = this.GetTable(data, this.StorageName);

			DataColumnCollection columns = table.Columns;
			if (!columns.Contains(StorageIdField))
			{
				DataColumn idcolumn = columns.Add(StorageIdField, typeof(int));
				idcolumn.AutoIncrementSeed = 1;
				idcolumn.AutoIncrementStep = 1;
				idcolumn.AutoIncrement = true;
			}
			table.PrimaryKey = new DataColumn[] { table.Columns[StorageIdField] };

			foreach (SavableField field in this.StorageFields)
			{
				if (!columns.Contains(field.Name))
				{
					columns.Add(field.Name, field.Type);
				}
			}
			return table;
		}

		protected internal DataTable GetTable(DataSet data, string name)
		{
			name = name.Replace("<", "-_").Replace(">", "_-").Replace("`", "_");
			if (!data.Tables.Contains(name))
			{
				data.Tables.Add(name);
			}
			DataTable table = data.Tables[name];
			return table;
		}

		/// <summary>
		/// creates a new list of fields and fills it...
		/// </summary>
		protected SavableFields CreateSavableFields()
		{
			if (this.StorageFields != null)
				return this.StorageFields;

			var fields = new SavableFields();
			this.SavableFields(fields);
			return this.StorageFields = fields;
		}
		/// <summary>
		/// add fields to be saved and loaded for the class...
		/// </summary>
		/// <param name="fields"></param>
		protected abstract void SavableFields(SavableFields fields);

		/// <summary>
		/// save this object to a dataset. will create new tables as required.
		/// </summary>
		/// <param name="data"></param>
		public int Save(DataSet data)
		{
			this.PrepareSave();
			DataTable table = this.CreateStorageTable(data);

			DataRow row = null;
			//if (this.StorageId != null)
			//{
			//	// check if our row already exists...
			//	row =
			//		(from r in table.AsEnumerable()
			//		 where (int?)r[StorageIdField] == this.StorageId
			//		 select r).FirstOrDefault();
			//}
			if (row == null)
			{
				row = table.NewRow();
				table.Rows.Add(row);
				this.StorageId = (int?)row[StorageIdField];
			}

			foreach (SavableField field in this.StorageFields)
			{
				row[field.Name] = field.Save(data);
			}
			this.CustomSave(data, table);

			return this.StorageId.Value;
		}

		protected virtual void PrepareSave()
		{
		}

		protected virtual void CustomSave(DataSet data, DataTable table)
		{
		}

		public object Load(DataSet data, int id)
		{
			this.PrepareLoad();

			this.StorageId = id;
			this.CreateSavableFields();

			DataTable table = this.GetTable(data, this.StorageName);
			DataRow row =
				(from r in table.AsEnumerable()
				 where SavableField<int>.Parse(r[StorageIdField]) == this.StorageId
				 select r).FirstOrDefault();

			if (row != null)
			{
				foreach (SavableField field in this.StorageFields)
				{
					object value = null;
					if (table.Columns.Contains(field.Name))
						value = row[field.Name];
					else
					{
						this.Log("Field '" + field.Name + "' not found in '" + this.StorageName + "'!");
						continue;
					}
					field.Load(data, value);
				}
			}

			this.CustomLoad(data, table);

			return this;
		}

		protected virtual void PrepareLoad()
		{
		}

		protected virtual void CustomLoad(DataSet data, DataTable table)
		{
		}

		public static T Load<T>(DataSet data, int id)
			where T : ISavable, new()
		{
			T instance = new T();
			instance.Load(data, id);
			return instance;
		}
	}
}
