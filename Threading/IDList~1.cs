using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Threading
{
	public class IDList<T>
		: IList<T>
	{
		protected System.Collections.Generic.List<T> Items = new System.Collections.Generic.List<T>();

		#region Constructors
		public IDList()
		{
		}

		public IDList(params T[] items)
		{
			this.AddRange(items);
		}
		#endregion


		public virtual int this[T item]
		{
			get
			{
				if (!this.Contains(item))
					this.Add(item);
				return this.IndexOf(item);
			}
		}

		#region IList<T> Members

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified
		/// predicate, and returns the first occurrence within the entire System.Collections.Generic.List<T>.		/// </summary>
		/// <param name="match">The System.Predicate<T> delegate that defines the conditions of the element to search for.</param>
		/// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type T.</returns>
		public T Find(Predicate<T> match)
		{
			return this.Items.Find(match);
		}

		public int IndexOf(T item)
		{
			return this.Items.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			this.Items.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			this.Items.RemoveAt(index);
		}

		public T this[int index]
		{
			get
			{
				return this.Items[index];
			}
			set
			{
				this.Items[index] = value;
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			this.Items.Add(item);
		}

		public void AddRange(params T[] items)
		{
			this.Items.AddRange(items);
		}
		public void AddRange(IEnumerable<T> items)
		{
			this.Items.AddRange(items);
		}

		public void Clear()
		{
			this.Items.Clear();
		}

		public bool Contains(T item)
		{
			return this.Items.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
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

		public bool Remove(T item)
		{
			return this.Items.Remove(item);
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion
	}
}
