#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag >>at<< zeroflag >>dot<< de
//	
//	Copyright (C) 2006-2009  Thomas "zeroflag" Kraemer
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
	[Serializable]
	public class List<T>
		: IList<T>,
		System.Collections.ICollection,
		System.Collections.IList
	{
		#region Constructors
		public List()
		{
		}

		public List( System.Collections.Generic.List<T> list )
			: this()
		{
			this._Items = list;
		}

		public List( IEnumerable<T> list )
			: this()
		{
			this._Items = new System.Collections.Generic.List<T>( list );
		}
		//public List( System.Collections.Generic.List<T> list )
		//    : this()
		//{
		//    this._Items = list;
		//}

		#endregion Constructors

		#region Helper Methods
		public List<T> Modify( Action<List<T>> modifier )
		{
			if ( modifier != null )
				modifier( this );
			return this;
		}
		#endregion Helper Methods

		#region event ItemAdded
		public delegate void ItemAddedHandler( T item );

		private event ItemAddedHandler _ItemAdded;
		/// <summary>
		/// Item added
		/// </summary>
		public virtual event ItemAddedHandler ItemAdded
		{
			add { this._ItemAdded += value; }
			remove { this._ItemAdded -= value; }
		}
		/// <summary>
		/// Call to raise the ItemAdded event:
		/// Item added
		/// </summary>
		protected virtual void OnItemAdded( T item )
		{
			// if there are event subscribers...
			if ( this._ItemAdded != null )
			{
				// call them...
				this._ItemAdded( item );
			}
		}
		#endregion event ItemAdded

		#region event ItemRemoved
		public delegate void ItemRemovedHandler( T item );

		private event ItemRemovedHandler _ItemRemoved;
		/// <summary>
		/// Item removed.
		/// </summary>
		public virtual event ItemRemovedHandler ItemRemoved
		{
			add { this._ItemRemoved += value; }
			remove { this._ItemRemoved -= value; }
		}
		/// <summary>
		/// Call to raise the ItemRemoved event:
		/// Item removed.
		/// </summary>
		protected virtual void OnItemRemoved( T item )
		{
			// if there are event subscribers...
			if ( this._ItemRemoved != null )
			{
				// call them...
				this._ItemRemoved( item );
			}
		}
		#endregion event ItemRemoved

		#region ItemChanged event
		public delegate void ItemChangedHandler( object sender, T oldvalue, T newvalue );

		private event ItemChangedHandler _ItemChanged;
		/// <summary>
		/// Occurs when any item changes. Also called when a item is added or removed.
		/// </summary>
		public virtual event ItemChangedHandler ItemChanged
		{
			add { this._ItemChanged += value; }
			remove { this._ItemChanged -= value; }
		}

		/// <summary>
		/// Raises the ItemChanged event.
		/// </summary>
		protected virtual void OnItemChanged( T oldvalue, T newvalue )
		{
			// if there are event subscribers...
			if ( this._ItemChanged != null )
			{
				// call them...
				this._ItemChanged( this, oldvalue, newvalue );
			}
		}
		#endregion ItemChanged event

		#region System.Collections.Generic.ICollection`1

		public virtual void Add( T item )
		{
			this.Items.Add( item );
			this.OnItemAdded( item );
			this.OnItemChanged( default( T ), item );
		}

		public virtual void AddRange( params T[] items )
		{
			foreach ( T item in items )
				this.Add( item );
		}

		public virtual void AddRange( IEnumerable<T> items )
		{
			foreach ( T item in items )
				this.Add( item );
		}

		public virtual void Clear()
		{
			while ( this.Count > 0 )
				this.Remove( this[0] );
			this.Items.Clear();
		}

		public virtual bool Contains( T item )
		{
			return this.Items.Contains( item );
		}

		public virtual void CopyTo( T[] array, int arrayIndex )
		{
			this.Items.CopyTo( array, arrayIndex );
		}

		public virtual bool Remove( T item )
		{
			try
			{
				return this.Items.Remove( item );
			}
			finally
			{
				this.OnItemRemoved( item );
				this.OnItemChanged( item, default( T ) );
			}
		}

		public virtual int Count
		{
			get { return this.Items.Count; }
		}

		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		#endregion System.Collections.Generic.ICollection`1

		#region System.Collections.Generic.IList`1

		public virtual int IndexOf( T item )
		{
			return this.Items.IndexOf( item );
		}

		public virtual void Insert( int index, T item )
		{
			this.Items.Insert( index, item );
			this.OnItemAdded( item );
			this.OnItemChanged( default( T ), item );
		}

		public virtual void RemoveAt( int index )
		{
			this.Remove( this[index] );
		}

		public virtual T this[int index]
		{
			get { return this.Items[index]; }
			set
			{
				T old = this.Items[index];
				if ( object.ReferenceEquals( null, old ) || object.ReferenceEquals( null, value ) ||
					old.Equals( value ) )
				{
					this.Items[index] = value;
					this.OnItemRemoved( old );
					this.OnItemChanged( old, value );
					this.OnItemAdded( value );
				}
			}
		}
		public virtual T[] this[int start, int end]
		{
			get
			{
				return this[start, end, 1];
			}
		}

		public virtual T[] this[int start, int end, int step]
		{
			get
			{
				if ( end == 0 )
					end = -1;
				if ( end < 0 )
					end = this.Count + end;
				if ( start < 0 )
					start = this.Count + start;
				if ( start > end && step > 0 )
					step = -step;
				T[] value = new T[( end - start ) / step];
				//if ( step == 1 )
				//{
				//    this.CopyTo( value, 0 );
				//}
				//else
				{
					for ( int i = start, j = 0; j < value.Length; i += step, j++ )
						value[j] = this[i];
				}
				return value;
			}
		}

		public T[] ToArray()
		{
			return this.Items.ToArray();
		}

		#endregion System.Collections.Generic.IList`1

		#region Searching

		/// <summary>
		/// Searches for an element that matches the conditions defined by the specified
		/// predicate, and returns the first occurrence within the entire System.Collections.Generic.List<T>.
		/// </summary>
		/// <param name="match">The System.Predicate<T> delegate that defines the conditions of the element to search for.</param>
		/// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, the default value for type T.</returns>
		public T Find( Predicate<T> match )
		{
#if !SILVERLIGHT
			return this.Items.Find( match );
#else
			if ( match == null )
			{
				throw new ArgumentNullException( "match" );
			}
			for ( int i = 0; i < this.Count; i++ )
			{
				if ( match( this.Items[i] ) )
				{
					return this.Items[i];
				}
			}
			return default( T );
#endif
		}

		/// <summary>
		/// Retrieves all the elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <param name="match">The System.Predicate<T> delegate that defines the conditions of the elements to search for.</param>
		/// <returns>A System.Collections.Generic.List<T> containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty List<T>.</returns>
		public List<T> FindAll( Predicate<T> match )
		{
#if !SILVERLIGHT
			return new List<T>( this.Items.FindAll( match ) );
#else
			if ( match == null )
			{
				throw new ArgumentNullException( "match" );
			}
			List<T> list = new List<T>();
			for ( int i = 0; i < this.Count; i++ )
			{
				if ( match( this.Items[i] ) )
				{
					list.Add( this.Items[i] );
				}
			}
			return list;
#endif
		}

		/// <summary>
		/// Searches for an element that matches the type specified.
		/// </summary>
		/// <typeparam name="S">The type of element to be found.</typeparam>
		/// <returns>The first element that matches the conditions defined by the specified type, if found; otherwise, the default value for type T.</returns>
		public S Find<S>()
			where S : T
		{
			try
			{
				return (S)this.Find( m => m != null && typeof( S ).IsAssignableFrom( m.GetType() ) );
			}
			catch ( InvalidCastException )
			{
				return default( S );
			}
		}

		

		/// <summary>
		/// Retrieves all the elements that match the the type specified.
		/// </summary>
		/// <typeparam name="S">The base-type to search for.</typeparam>
		/// <returns>A System.Collections.Generic.List<T> containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty List<T>.</returns>
		public List<S> FindAll<S>()
			where S : T
		{
			var results = new List<S>();
			foreach ( T val in this.FindAll( m => m != null && typeof( S ).IsAssignableFrom( m.GetType() ) ) )
			{
				try
				{
					results.Add( (S)val );
				}
				catch ( InvalidCastException )
				{
				}
			}
			return results;
		}

		#endregion

		#region Sort
		/// <summary>
		/// Sorts the elements in the entire System.Collections.Generic.List<T> using the default comparer.
		/// </summary>
		public virtual void Sort()
		{
			this.Items.Sort();
		}

		/// <summary>
		/// Sorts the elements in the entire System.Collections.Generic.List<T> using the specified System.Comparison<T>.
		/// </summary>
		/// <param name="comparison">The System.Comparison<T> to use when comparing elements.</param>
		public void Sort( Comparison<T> comparison )
		{
			this.Items.Sort( comparison );
		}

		/// <summary>
		/// Sorts the elements in the entire System.Collections.Generic.List<T> using the specified comparer.
		/// </summary>
		/// <param name="comparer">The System.Collections.Generic.IComparer<T> implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer<T>.Default.</param>
		public void Sort( IComparer<T> comparer )
		{
			this.Items.Sort( comparer );
		}


		/// <summary>
		/// Sorts the elements in a range of elements in System.Collections.Generic.List<T> using the specified comparer.
		/// </summary>
		/// <param name="index">The zero-based starting index of the range to sort.</param>
		/// <param name="count">The length of the range to sort.</param>
		/// <param name="comparer">The System.Collections.Generic.IComparer<T> implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer<T>.Default.</param>
		public void Sort( int index, int count, IComparer<T> comparer )
		{
			this.Items.Sort( index, count, comparer );
		}
		#endregion Sort

		#region Values
		private System.Collections.Generic.List<T> _Items = new System.Collections.Generic.List<T>();

		protected System.Collections.Generic.List<T> Items
		{
			get { return _Items; }
			//set { _Values = value; }
		}
		#endregion Values

		#region System.Collections.IEnumerable

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		#endregion System.Collections.IEnumerable


		#region System.Collections.Generic.IEnumerable`1

		public virtual System.Collections.Generic.IEnumerator<T> GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		#endregion System.Collections.Generic.IEnumerable`1


		#region ICollection Members

		void System.Collections.ICollection.CopyTo( Array array, int index )
		{
			this.CopyTo( (T[])array, index );
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get { return false; }
		}

		object System.Collections.ICollection.SyncRoot
		{
			get { return null; }
		}

		#endregion

		#region IList Members

		int System.Collections.IList.Add( object value )
		{
			T item = (T)value;
			this.Add( item );
			return this.IndexOf( item );
		}

		bool System.Collections.IList.Contains( object value )
		{
			return this.Contains( (T)value );
		}

		int System.Collections.IList.IndexOf( object value )
		{
			return this.IndexOf( (T)value );
		}

		void System.Collections.IList.Insert( int index, object value )
		{
			this.Insert( index, (T)value );
		}

		void System.Collections.IList.Remove( object value )
		{
			this.Remove( (T)value );
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

		bool System.Collections.IList.IsFixedSize
		{
			get { return false; }
		}

		#endregion
	}

}
