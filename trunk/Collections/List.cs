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
	[Serializable]
	public class List<T> : IList<T>
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
				this.Remove( this[ 0 ] );
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
			this.Remove( this[ index ] );
		}

		public virtual T this[ int index ]
		{
			get { return this.Items[ index ]; }
			set
			{
				T old = this.Items[ index ];
				if ( object.ReferenceEquals( null, old ) || object.ReferenceEquals( null, value ) ||
					old.Equals( value ) )
				{
					this.Items[ index ] = value;
					this.OnItemRemoved( old );
					this.OnItemChanged( old, value );
					this.OnItemAdded( value );
				}
			}
		}
		public virtual T[] this[ int start, int end ]
		{
			get
			{
				return this[ start, end, 1 ];
			}
		}

		public virtual T[] this[ int start, int end, int step ]
		{
			get
			{
				if ( end < 0 )
					end = this.Count + end;
				if ( start < 0 )
					start = this.Count + start;
				if ( start > end && step > 0 )
					step = -step;
				T[] value = new T[ ( end - start ) / step ];
				if ( step == 1 )
				{
					this.CopyTo( value, 0 );
				}
				else
				{
					for ( int i = start, j = 0; j < value.Length; i += step, j++ )
						value[ j ] = this[ i ];
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
			return this.Items.Find( match );
		}

		/// <summary>
		/// Retrieves all the elements that match the conditions defined by the specified predicate.
		/// </summary>
		/// <param name="match">The System.Predicate<T> delegate that defines the conditions of the elements to search for.</param>
		/// <returns>A System.Collections.Generic.List<T> containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty System.Collections.Generic.List<T>.</returns>
		public List<T> FindAll( Predicate<T> match )
		{
			return new List<T>( this.Items.FindAll( match ) );
		}

		#endregion

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

	}

}
