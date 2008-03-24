using System;
using System.Collections.Generic;
using System.Collections;

namespace zeroflag.Collections
{
	/// <summary>
	/// This is a list that also implemens ICollection. (useful for the winforms designer as it doesn't support generic collections)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Collection2<T>
		:
		zeroflag.Collections.List<T>,
		IList<T>,
		IList,
		ICollection
	{
		#region IList Members

		int IList.Add(object value)
		{
			base.Add((T)value);
			return this.IndexOf((T)value);
		}

		bool IList.Contains(object value)
		{
			return base.Contains((T)value);
		}

		int IList.IndexOf(object value)
		{
			return base.IndexOf((T)value);
		}

		void IList.Insert(int index, object value)
		{
			base.Insert(index, (T)value);
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		void IList.Remove(object value)
		{
			base.Remove((T)value);
		}

		object IList.this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				base[index] = (T)value;
			}
		}

		#endregion

		#region ICollection Members

		void ICollection.CopyTo(Array array, int index)
		{
			try
			{
				base.CopyTo((T[])array, index);
			}
			catch (InvalidCastException)
			{
				for (int i = 0; i < this.Count; i++)
				{
					array.SetValue(this[i], i + index);
				}
			}
		}
		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		object ICollection.SyncRoot
		{
			get { return null; }
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return base.GetEnumerator();
		}

		#endregion
	}
}
