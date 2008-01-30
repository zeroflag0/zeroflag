#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2008  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************
#endregion LGPL License

using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Collections
{
	public class List<T> : IList<T>
	{
		#region Constructors
		public List()
		{
		}

		public List(IList<T> list)
			: this()
		{
			this._Values = list;
		}

		public List(IEnumerable<T> list)
			: this()
		{
			this.Values = new System.Collections.Generic.List<T>();
		}

		#endregion Constructors

		#region event ItemAdded
		public delegate void ItemAddedHandler(T item);

		private event ItemAddedHandler m_ItemAdded;
		/// <summary>
		/// Item added
		/// </summary>
		public virtual event ItemAddedHandler ItemAdded
		{
			add { this.m_ItemAdded += value; }
			remove { this.m_ItemAdded -= value; }
		}
		/// <summary>
		/// Call to raise the ItemAdded event:
		/// Item added
		/// </summary>
		protected virtual void OnItemAdded(T item)
		{
			// if there are event subscribers...
			if (this.m_ItemAdded != null)
			{
				// call them...
				this.m_ItemAdded(item);
			}
		}
		#endregion event ItemAdded

		#region event ItemRemoved
		public delegate void ItemRemovedHandler(T item);

		private event ItemRemovedHandler m_ItemRemoved;
		/// <summary>
		/// Item removed.
		/// </summary>
		public virtual event ItemRemovedHandler ItemRemoved
		{
			add { this.m_ItemRemoved += value; }
			remove { this.m_ItemRemoved -= value; }
		}
		/// <summary>
		/// Call to raise the ItemRemoved event:
		/// Item removed.
		/// </summary>
		protected virtual void OnItemRemoved(T item)
		{
			// if there are event subscribers...
			if (this.m_ItemRemoved != null)
			{
				// call them...
				this.m_ItemRemoved(item);
			}
		}
		#endregion event ItemRemoved

		#region ItemChanged event
		public delegate void ItemChangedHandler(object sender, T oldvalue, T newvalue);

		private event ItemChangedHandler m_ItemChanged;
		/// <summary>
		/// Occurs when any item changes. Also called when a item is added or removed.
		/// </summary>
		public virtual event ItemChangedHandler ItemChanged
		{
			add { this.m_ItemChanged += value; }
			remove { this.m_ItemChanged -= value; }
		}

		/// <summary>
		/// Raises the ItemChanged event.
		/// </summary>
		protected virtual void OnItemChanged(T oldvalue, T newvalue)
		{
			// if there are event subscribers...
			if (this.m_ItemChanged != null)
			{
				// call them...
				this.m_ItemChanged(this, oldvalue, newvalue);
			}
		}
		#endregion ItemChanged event

		#region System.Collections.Generic.ICollection`1

		public virtual void Add(T item)
		{
			this.Values.Add(item);
			this.OnItemAdded(item);
			this.OnItemChanged(default(T), item);
		}

		public virtual void Clear()
		{
			foreach (T item in this)
			{
				this.Remove(item);
			}
			this.Values.Clear();
		}

		public virtual bool Contains(T item)
		{
			return this.Values.Contains(item);
		}

		public virtual void CopyTo(T[] array, int arrayIndex)
		{
			this.Values.CopyTo(array, arrayIndex);
		}

		public virtual bool Remove(T item)
		{
			try
			{
				return this.Values.Remove(item);
			}
			finally
			{
				this.OnItemRemoved(item);
				this.OnItemChanged(item, default(T));
			}
		}

		public virtual int Count
		{
			get { return this.Values.Count; }
		}

		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		#endregion System.Collections.Generic.ICollection`1

		#region System.Collections.Generic.IList`1

		public virtual int IndexOf(T item)
		{
			return this.Values.IndexOf(item);
		}

		public virtual void Insert(int index, T item)
		{
			this.Values.Insert(index, item);
			this.OnItemAdded(item);
			this.OnItemChanged(default(T), item);
		}

		public virtual void RemoveAt(int index)
		{
			this.Remove(this[index]);
		}

		public virtual T this[int index]
		{
			get { return this.Values[index]; }
			set
			{
				T old = this.Values[index];
				if (object.ReferenceEquals(null, old) || object.ReferenceEquals(null, value) ||
					old.Equals(value))
				{
					this.Values[index] = value;
					this.OnItemRemoved(old);
					this.OnItemChanged(old, value);
					this.OnItemAdded(value);
				}
			}
		}

		#endregion System.Collections.Generic.IList`1

		#region Values
		private System.Collections.Generic.IList<T> _Values = new System.Collections.Generic.List<T>();

		protected System.Collections.Generic.IList<T> Values
		{
			get { return _Values; }
			set { _Values = value; }
		}
		#endregion Values

		#region System.Collections.IEnumerable

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.Values.GetEnumerator();
		}

		#endregion System.Collections.IEnumerable


		#region System.Collections.Generic.IEnumerable`1

		public virtual System.Collections.Generic.IEnumerator<T> GetEnumerator()
		{
			return this.Values.GetEnumerator();
		}

		#endregion System.Collections.Generic.IEnumerable`1

	}

}
