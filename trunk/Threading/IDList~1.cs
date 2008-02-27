using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Threading
{
	public class IDList<T>
		: IList<T>
	{
		protected System.Collections.Generic.List<T> Items = new System.Collections.Generic.List<T>();




		#region System.Collections.IEnumerable

		public virtual System.Collections.IEnumerator GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		#endregion System.Collections.IEnumerable


		#region System.Collections.Generic.IEnumerable`1

		public virtual System.Collections.Generic.IEnumerator<T> GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		public virtual System.Collections.Generic.IEnumerator<T> GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		#endregion System.Collections.Generic.IEnumerable`1


		#region System.Collections.Generic.ICollection`1

		public virtual void Add(T item)
		{
			this.Items.Add(item);
		}

		public virtual void Clear()
		{
			this.Items.Clear();
		}

		public virtual bool Contains(T item)
		{
			return this.Items.Contains(item);
		}

		public virtual void CopyTo(T[] array, int arrayIndex)
		{
			this.Items.CopyTo(array, arrayIndex);
		}

		public virtual bool Remove(T item)
		{
			return this.Items.Remove(item);
		}

		public virtual int Count
		{
			get { return this.Items.Count; }
		}

		public virtual bool IsReadOnly
		{
			get { return this.Items.IsReadOnly; }
		}

		public virtual void Add(T item)
		{
			this.Items.Add(item);
		}

		public virtual void Clear()
		{
			this.Items.Clear();
		}

		public virtual bool Contains(T item)
		{
			return this.Items.Contains(item);
		}

		public virtual void CopyTo(T[] array, int arrayIndex)
		{
			this.Items.CopyTo(array, arrayIndex);
		}

		public virtual bool Remove(T item)
		{
			return this.Items.Remove(item);
		}

		public virtual int Count
		{
			get { return this.Items.Count; }
		}

		public virtual bool IsReadOnly
		{
			get { return this.Items.IsReadOnly; }
		}

		#endregion System.Collections.Generic.ICollection`1


		#region System.Collections.Generic.IList`1

		public virtual int IndexOf(T item)
		{
			return this.Items.IndexOf(item);
		}

		public virtual void Insert(int index, T item)
		{
			this.Items.Insert(index, item);
		}

		public virtual void RemoveAt(int index)
		{
			this.Items.RemoveAt(index);
		}

		public virtual T this[int index]
		{
			get { return this.Items[index]; }
			set { this.Items[index] = value; }
		}

		public virtual int IndexOf(T item)
		{
			return this.Items.IndexOf(item);
		}

		public virtual void Insert(int index, T item)
		{
			this.Items.Insert(index, item);
		}

		public virtual void RemoveAt(int index)
		{
			this.Items.RemoveAt(index);
		}

		public virtual T this[int index]
		{
			get { return this.Items[index]; }
			set { this.Items[index] = value; }
		}

		#endregion System.Collections.Generic.IList`1


		#region System.Collections.ICollection

		public virtual void CopyTo(System.Array array, int index)
		{
			this.Items.CopyTo(array, index);
		}

		public virtual int Count
		{
			get { return this.Items.Count; }
		}

		public virtual System.Object SyncRoot
		{
			get { return this.Items.SyncRoot; }
		}

		public virtual bool IsSynchronized
		{
			get { return this.Items.IsSynchronized; }
		}

		#endregion System.Collections.ICollection


		#region System.Collections.IList

		public virtual int Add(System.Object value)
		{
			return this.Items.Add(value);
		}

		public virtual bool Contains(System.Object value)
		{
			return this.Items.Contains(value);
		}

		public virtual void Clear()
		{
			this.Items.Clear();
		}

		public virtual int IndexOf(System.Object value)
		{
			return this.Items.IndexOf(value);
		}

		public virtual void Insert(int index, System.Object value)
		{
			this.Items.Insert(index, value);
		}

		public virtual void Remove(System.Object value)
		{
			this.Items.Remove(value);
		}

		public virtual void RemoveAt(int index)
		{
			this.Items.RemoveAt(index);
		}

		public virtual System.Object this[int index]
		{
			get { return this.Items[index]; }
			set { this.Items[index] = value; }
		}

		public virtual bool IsReadOnly
		{
			get { return this.Items.IsReadOnly; }
		}

		public virtual bool IsFixedSize
		{
			get { return this.Items.IsFixedSize; }
		}

		#endregion System.Collections.IList


		#region System.Collections.Generic.List`1

		public System.Collections.Generic.List<T> ConvertAll<T>(System.Converter<T, T2> converter)
		{
			return this.Items.ConvertAll(converter);
		}

		public virtual void Add(T item)
		{
			this.Items.Add(item);
		}

		public void AddRange(System.Collections.Generic.IEnumerable<T> collection)
		{
			this.Items.AddRange(collection);
		}

		public System.Collections.ObjectModel.ReadOnlyCollection<T> AsReadOnly()
		{
			return this.Items.AsReadOnly();
		}

		public int BinarySearch(int index, int count, T item, System.Collections.Generic.IComparer<T> comparer)
		{
			return this.Items.BinarySearch(index, count, item, comparer);
		}

		public int BinarySearch(T item)
		{
			return this.Items.BinarySearch(item);
		}

		public int BinarySearch(T item, System.Collections.Generic.IComparer<T> comparer)
		{
			return this.Items.BinarySearch(item, comparer);
		}

		public virtual void Clear()
		{
			this.Items.Clear();
		}

		public virtual bool Contains(T item)
		{
			return this.Items.Contains(item);
		}

		public void CopyTo(T[] array)
		{
			this.Items.CopyTo(array);
		}

		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			this.Items.CopyTo(index, array, arrayIndex, count);
		}

		public virtual void CopyTo(T[] array, int arrayIndex)
		{
			this.Items.CopyTo(array, arrayIndex);
		}

		public bool Exists(System.Predicate<T> match)
		{
			return this.Items.Exists(match);
		}

		public T Find(System.Predicate<T> match)
		{
			return this.Items.Find(match);
		}

		public System.Collections.Generic.List<T> FindAll(System.Predicate<T> match)
		{
			return this.Items.FindAll(match);
		}

		public int FindIndex(System.Predicate<T> match)
		{
			return this.Items.FindIndex(match);
		}

		public int FindIndex(int startIndex, System.Predicate<T> match)
		{
			return this.Items.FindIndex(startIndex, match);
		}

		public int FindIndex(int startIndex, int count, System.Predicate<T> match)
		{
			return this.Items.FindIndex(startIndex, count, match);
		}

		public T FindLast(System.Predicate<T> match)
		{
			return this.Items.FindLast(match);
		}

		public int FindLastIndex(System.Predicate<T> match)
		{
			return this.Items.FindLastIndex(match);
		}

		public int FindLastIndex(int startIndex, System.Predicate<T> match)
		{
			return this.Items.FindLastIndex(startIndex, match);
		}

		public int FindLastIndex(int startIndex, int count, System.Predicate<T> match)
		{
			return this.Items.FindLastIndex(startIndex, count, match);
		}

		public void ForEach(System.Action<T> action)
		{
			this.Items.ForEach(action);
		}

		public System.Collections.Generic.Enumerator<T> GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		public System.Collections.Generic.List<T> GetRange(int index, int count)
		{
			return this.Items.GetRange(index, count);
		}

		public virtual int IndexOf(T item)
		{
			return this.Items.IndexOf(item);
		}

		public int IndexOf(T item, int index)
		{
			return this.Items.IndexOf(item, index);
		}

		public int IndexOf(T item, int index, int count)
		{
			return this.Items.IndexOf(item, index, count);
		}

		public virtual void Insert(int index, T item)
		{
			this.Items.Insert(index, item);
		}

		public void InsertRange(int index, System.Collections.Generic.IEnumerable<T> collection)
		{
			this.Items.InsertRange(index, collection);
		}

		public int LastIndexOf(T item)
		{
			return this.Items.LastIndexOf(item);
		}

		public int LastIndexOf(T item, int index)
		{
			return this.Items.LastIndexOf(item, index);
		}

		public int LastIndexOf(T item, int index, int count)
		{
			return this.Items.LastIndexOf(item, index, count);
		}

		public virtual bool Remove(T item)
		{
			return this.Items.Remove(item);
		}

		public int RemoveAll(System.Predicate<T> match)
		{
			return this.Items.RemoveAll(match);
		}

		public virtual void RemoveAt(int index)
		{
			this.Items.RemoveAt(index);
		}

		public void RemoveRange(int index, int count)
		{
			this.Items.RemoveRange(index, count);
		}

		public void Reverse()
		{
			this.Items.Reverse();
		}

		public void Reverse(int index, int count)
		{
			this.Items.Reverse(index, count);
		}

		public void Sort()
		{
			this.Items.Sort();
		}

		public void Sort(System.Collections.Generic.IComparer<T> comparer)
		{
			this.Items.Sort(comparer);
		}

		public void Sort(int index, int count, System.Collections.Generic.IComparer<T> comparer)
		{
			this.Items.Sort(index, count, comparer);
		}

		public void Sort(System.Comparison<T> comparison)
		{
			this.Items.Sort(comparison);
		}

		public T[] ToArray()
		{
			return this.Items.ToArray();
		}

		public void TrimExcess()
		{
			this.Items.TrimExcess();
		}

		public bool TrueForAll(System.Predicate<T> match)
		{
			return this.Items.TrueForAll(match);
		}

		public System.Type GetType()
		{
			return this.Items.GetType();
		}

		public virtual System.String ToString()
		{
			return this.Items.ToString();
		}

		public virtual bool Equals(System.Object obj)
		{
			return this.Items.Equals(obj);
		}

		public virtual int GetHashCode()
		{
			return this.Items.GetHashCode();
		}

		public IDList()
		{
			this.Items = new System.Collections.Generic.List<T>();
		}

		public IDList(int capacity)
		{
			this.Items = new System.Collections.Generic.List<T>(capacity);
		}

		public IDList(System.Collections.Generic.IEnumerable<T> collection)
		{
			this.Items = new System.Collections.Generic.List<T>(collection);
		}

		public int Capacity
		{
			get { return this.Items.Capacity; }
			set { this.Items.Capacity = value; }
		}

		public virtual int Count
		{
			get { return this.Items.Count; }
		}

		public virtual T this[int index]
		{
			get { return this.Items[index]; }
			set { this.Items[index] = value; }
		}

		#endregion System.Collections.Generic.List`1

		public virtual int this[T item]
		{
			get
			{
				if (!this.Contains(item))
					this.Add(item);
				return this.IndexOf(item);
			}
		}
	}
}
