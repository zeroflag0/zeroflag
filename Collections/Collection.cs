using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Collections
{
	[System.ComponentModel.ListBindable( false )]
	[System.Serializable]
	[System.ComponentModel.TypeConverter( typeof( System.ComponentModel.CollectionConverter ) )]
	public class Collection<T>
		: List<T>
		, System.Collections.Generic.IList<T>
		, System.ICloneable
	{
		//private zeroflag.Collections.List<T> _Items = new zeroflag.Collections.List<T>();

		//protected zeroflag.Collections.List<T> Items
		//{
		//    get { return _Items; }
		//}

		#region zeroflag.Collections.List`1

		//public event zeroflag.Collections.List<T>.ItemAddedHandler ItemAdded
		//{
		//    add { base.ItemAdded += value; }
		//    remove { base.ItemAdded -= value; }
		//}

		//public event zeroflag.Collections.List<T>.ItemRemovedHandler ItemRemoved
		//{
		//    add { base.ItemRemoved += value; }
		//    remove { base.ItemRemoved -= value; }
		//}

		//public event zeroflag.Collections.List<T>.ItemChangedHandler ItemChanged
		//{
		//    add { base.ItemChanged += value; }
		//    remove { base.ItemChanged -= value; }
		//}

		public Collection<T> Modify( System.Action<Collection<T>> modifier )
		{
			if ( modifier != null )
				modifier( this );

			return this;
		}

		//public T[] ToArray()
		//{
		//    return base.ToArray();
		//}

		#endregion zeroflag.Collections.List`1

		#region System.Collections.Generic.ICollection`1

		public virtual int Count
		{
			get { return base.Count; }
		}

		public virtual bool IsReadOnly
		{
			get { return base.IsReadOnly; }
		}

		public virtual void Add( T item )
		{
			base.Add( item );
		}

		public virtual void AddRange( T[] items )
		{
			foreach ( T item in items )
				this.Add( item );
		}

		public virtual void AddRange( IEnumerable<T> items )
		{
			foreach ( T item in items )
				this.Add( item );
		}

		public virtual void AddRange( System.Collections.IEnumerable items )
		{
			foreach ( T item in items )
				this.Add( item );
		}

		public virtual void Clear()
		{
			base.Clear();
		}

		public virtual bool Contains( T item )
		{
			return base.Contains( item );
		}

		public virtual void CopyTo( T[] array, int arrayIndex )
		{
			base.CopyTo( array, arrayIndex );
		}

		public virtual bool Remove( T item )
		{
			return base.Remove( item );
		}

		#endregion System.Collections.Generic.ICollection`1


		#region System.Collections.Generic.IList`1

		public virtual T this[ int index ]
		{
			get { return base[ index ]; }
			set { base[ index ] = value; }
		}

		public virtual int IndexOf( T item )
		{
			return base.IndexOf( item );
		}

		public virtual void Insert( int index, T item )
		{
			base.Insert( index, item );
		}

		public virtual void RemoveAt( int index )
		{
			base.RemoveAt( index );
		}

		#endregion System.Collections.Generic.IList`1

		#region System.ICloneable

		public virtual System.Object Clone()
		{
			Collection<T> clone = new Collection<T>();
			foreach ( T item in this )
			{
				try
				{
					if ( item is ICloneable )
					{
						clone.Add( (T)( (ICloneable)item ).Clone() );
						continue;
					}
				}
				catch { }
				clone.Add( item );
			}

			return clone;
		}

		#endregion System.ICloneable

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return base.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		public Collection()
			: base()
		{
		}

		public Collection( System.Collections.Generic.List<T> list )
			: base( list )
		{
		}

		public Collection( IEnumerable<T> list )
			: base( list )
		{
		}
	}
}
