﻿using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Collections
{
	[System.ComponentModel.ListBindable(false)]
	[System.Serializable]
	[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.CollectionConverter))]
	public class Collection<T>
		: System.Collections.IList
		, System.Collections.Generic.IList<T>
		, System.Collections.ICollection
		, System.ICloneable
	{
		protected zeroflag.Collections.List<T> Items = new zeroflag.Collections.List<T>();

		#region zeroflag.Collections.List`1

		public event zeroflag.Collections.List<T>.ItemAddedHandler ItemAdded
		{
			add { this.Items.ItemAdded += value; }
			remove { this.Items.ItemAdded -= value; }
		}

		public event zeroflag.Collections.List<T>.ItemRemovedHandler ItemRemoved
		{
			add { this.Items.ItemRemoved += value; }
			remove { this.Items.ItemRemoved -= value; }
		}

		public event zeroflag.Collections.List<T>.ItemChangedHandler ItemChanged
		{
			add { this.Items.ItemChanged += value; }
			remove { this.Items.ItemChanged -= value; }
		}

		public Collection<T> Modify(System.Action<Collection<T>> modifier)
		{
			if (modifier != null)
				modifier(this);

			return this;
		}

		#endregion zeroflag.Collections.List`1

		#region System.Collections.Generic.ICollection`1

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

		public virtual void AddRange(T[] items)
		{
			foreach (T item in items)
				this.Add(item);
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

		#endregion System.Collections.Generic.ICollection`1


		#region System.Collections.Generic.IList`1

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

		#endregion System.Collections.Generic.IList`1


		#region System.Collections.ICollection

		public virtual System.Object SyncRoot
		{
			get { return null; }
		}

		public virtual bool IsSynchronized
		{
			get { return false; }
		}

		#endregion System.Collections.ICollection


		#region System.Collections.IList

		public virtual bool IsFixedSize
		{
			get { return false; }
		}

		#endregion System.Collections.IList


		#region System.ICloneable

		public virtual System.Object Clone()
		{
			Collection<T> clone = new Collection<T>();
			foreach (T item in this)
			{
				try
				{
					if (item is ICloneable)
					{
						clone.Add((T)((ICloneable)item).Clone());
						continue;
					}
				}
				catch { }
				clone.Add(item);
			}

			return clone;
		}

		#endregion System.ICloneable




		#region IList Members

		public int Add(object value)
		{
			T item = (T)value;
			this.Add(item);
			return this.IndexOf(item);
		}

		public bool Contains(object value)
		{
			return this.Contains((T)value);
		}

		public int IndexOf(object value)
		{
			return this.IndexOf((T)value);
		}

		public void Insert(int index, object value)
		{
			this.Insert(index, (T)value);
		}

		public void Remove(object value)
		{
			this.Remove((T)value);
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (T)value;
			}
		}

		#endregion

		#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			this.CopyTo((T[])array, index);
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
