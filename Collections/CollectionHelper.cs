using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Collections
{
	public class CollectionHelper<T> : IList<T>
	{
		private System.Collections.IList _Base;

		public System.Collections.IList Base
		{
			get { return _Base; }
			set { _Base = value; }
		}

		public CollectionHelper()
		{
		}

		public CollectionHelper(System.Collections.IList collection)
		{
			this.Base = collection;
		}


		#region System.Collections.Generic.ICollection`1

		public virtual int Count
		{
			get { return this.Base.Count; }
		}

		public virtual bool IsReadOnly
		{
			get { return this.Base.IsReadOnly; }
		}

		public virtual void Add(T item)
		{
			this.Base.Add(item);
		}

		public virtual void Clear()
		{
			this.Base.Clear();
		}

		public virtual bool Contains(T item)
		{
			return this.Base.Contains(item);
		}

		public virtual void CopyTo(T[] array, int arrayIndex)
		{
			this.Base.CopyTo(array, arrayIndex);
		}

		public virtual bool Remove(T item)
		{
			return this.Base.Remove(item);
		}

		#endregion System.Collections.Generic.ICollection`1

		#region System.Collections.Generic.IList`1

		public virtual T this[int index]
		{
			get { return this.Base[index]; }
			set { this.Base[index] = value; }
		}

		public virtual int IndexOf(T item)
		{
			return this.Base.IndexOf(item);
		}

		public virtual void Insert(int index, T item)
		{
			this.Base.Insert(index, item);
		}

		public virtual void RemoveAt(int index)
		{
			this.Base.RemoveAt(index);
		}

		#endregion System.Collections.Generic.IList`1

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return this.EnumerationHelper().GetEnumerator();
		}

		protected IEnumerable<T> EnumerationHelper()
		{
			foreach (object o in this.Base)
			{
				yield return (T)o;
			}
		}
		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.Base.GetEnumerator();
		}

		#endregion
	}
}
