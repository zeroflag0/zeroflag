using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace Basics.Components
{
	public class SavableFields : ICollection<SavableField>
	{
		List<SavableField> _Items = new List<SavableField>();
		protected List<SavableField> Items { get { return _Items; } }

		public void Add<T>(string name, Func<DataSet, T> save, Action<DataSet, T> load)
		{
			this.Add(new SavableField<T>(name, save, load));
		}

		public void Add<T>(string name, Func<T> save, Action<DataSet, T> load)
		{
			this.Add(new SavableField<T>(name, (data) => save(), load));
		}

		public void Add<T>(string name, Func<DataSet, T> save, Action<T> load)
		{
			this.Add(new SavableField<T>(name, save, (data, value) => load(value)));
		}

		public void Add<T>(string name, Func<T> save, Action<T> load)
		{
			this.Add(new SavableField<T>(name, (data) => save(), (data, value) => load(value)));
		}

		public void Add(SavableField item)
		{
			this.Items.Add(item);
		}

		public void Clear()
		{
			this.Items.Clear();
		}

		public bool Contains(SavableField item)
		{
			return this.Items.Contains(item);
		}

		public void CopyTo(SavableField[] array, int arrayIndex)
		{
			this.Items.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return this.Items.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(SavableField item)
		{
			return this.Items.Remove(item);
		}

		public IEnumerator<SavableField> GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}
