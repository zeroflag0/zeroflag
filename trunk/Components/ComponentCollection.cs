#region BSD license
/*
 * Copyright (c) 2008, Thomas "zeroflag" Kraemer. All rights reserved.
 * Copyright (c) 2008, Anders "anonimasu" Helin. All rights reserved.
 * Copyright (c) 2008, The zeroflag.Components.NET Team. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without 
 * modification, are permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * Redistributions in binary form must reproduce the above copyright notice, 
 * this list of conditions and the following disclaimer in the documentation 
 * and/or other materials provided with the distribution.
 * Neither the name of the zeroflag.Components.NET Team nor the names of its contributors may 
 * be used to endorse or promote products derived from this software without 
 * specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF
 * THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion BSD license

#region SVN Version Information
///	<file>
///		<!-- Last modification of this file: -->
///		<revision>$Rev: 55 $</revision>
///		<author>$Author: zeroflag $</author>
///		<id>$Id: BoundingBox.cs 55 2008-11-24 15:25:03Z zeroflag $</id>
///	</file>
#endregion SVN Version Information

using System;
using System.Linq;
using System.Text;
using zeroflag.Collections;

namespace zeroflag.Components
{
	public class ComponentCollection<T> :
		Component,
		System.Collections.Generic.IList<T>,
		System.Collections.ICollection,
		System.Collections.IList
		where T : Component
	{
		public ComponentCollection( Component owner )
		{
			this.Outer = owner;
		}

		protected override void OnDispose()
		{
		}

		#region Helper Methods
		public ComponentCollection<T> Modify( Action<ComponentCollection<T>> modifier )
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

		#region event ItemAdded
		///// <summary>
		///// Item added
		///// </summary>
		//public virtual event ItemAddedHandler ItemAdded
		//{
		//    add { this.Inner.ItemAdded += value; }
		//    remove { this.Inner.ItemAdded -= value; }
		//}
		#endregion event ItemAdded

		#region event ItemRemoved
		///// <summary>
		///// Item removed.
		///// </summary>
		//public virtual event ItemRemovedHandler ItemRemoved
		//{
		//    add { this.Inner.ItemRemoved += value; }
		//    remove { this.Inner.ItemRemoved -= value; }
		//}
		#endregion event ItemRemoved

		#region ItemChanged event
		///// <summary>
		///// Occurs when any item changes. Also called when a item is added or removed.
		///// </summary>
		//public virtual event ItemChangedHandler ItemChanged
		//{
		//    add { this.Inner.ItemChanged += value; }
		//    remove { this.Inner.ItemChanged -= value; }
		//}
		#endregion ItemChanged event

		#region System.Collections.Generic.ICollection`1

		public virtual void Add( T item )
		{
			this.Items.Add( item );
		}

		public virtual void AddRange( params T[] items )
		{
			this.Items.AddRange( items );
		}

		public virtual void AddRange( System.Collections.Generic.IEnumerable<T> items )
		{
			this.Items.AddRange( items );
		}

		public virtual void Clear()
		{
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
			return this.Items.Remove( item );
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
		}

		public virtual void RemoveAt( int index )
		{
			this.Items.RemoveAt( index );
		}

		public virtual T this[int index]
		{
			get { return this.Items[index]; }
			set
			{
				this.Items[index] = value;
			}
		}
		public virtual T[] this[int start, int end]
		{
			get
			{
				return this.Items[start, end];
			}
		}

		public virtual T[] this[int start, int end, int step]
		{
			get
			{
				return this.Items[start, end, step];
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
			return this.Items.FindAll( match );
		}

		/// <summary>
		/// Searches for an element that matches the type specified.
		/// </summary>
		/// <typeparam name="S">The type of element to be found.</typeparam>
		/// <returns>The first element that matches the conditions defined by the specified type, if found; otherwise, the default value for type T.</returns>
		public S Find<S>()
			where S : T
		{
			return this.Items.Find<S>();
		}

		/// <summary>
		/// Retrieves all the elements that match the the type specified.
		/// </summary>
		/// <param name="match">The System.Predicate<T> delegate that defines the conditions of the elements to search for.</param>
		/// <returns>A System.Collections.Generic.List<T> containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty System.Collections.Generic.List<T>.</returns>
		public List<S> FindAll<S>()
			where S : T
		{
			return this.Items.FindAll<S>();
		}

		#endregion

		#region Sort
		/// <summary>
		/// Sorts the elements in the entire System.Collections.Generic.List using the default comparer.
		/// </summary>
		public virtual void Sort()
		{
			this.Items.Sort();
		}

		/// <summary>
		/// Sorts the elements in the entire System.Collections.Generic.List using the specified System.Comparison.
		/// </summary>
		/// <param name="comparison">The System.Comparison to use when comparing elements.</param>
		public void Sort( Comparison<T> comparison )
		{
			this.Items.Sort( comparison );
		}

		/// <summary>
		/// Sorts the elements in the entire System.Collections.Generic.List using the specified comparer.
		/// </summary>
		/// <param name="comparer">The System.Collections.Generic.IComparer implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer.Default.</param>
		public void Sort( System.Collections.Generic.IComparer<T> comparer )
		{
			this.Items.Sort( comparer );
		}


		/// <summary>
		/// Sorts the elements in a range of elements in System.Collections.Generic.List using the specified comparer.
		/// </summary>
		/// <param name="index">The zero-based starting index of the range to sort.</param>
		/// <param name="count">The length of the range to sort.</param>
		/// <param name="comparer">The System.Collections.Generic.IComparer implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer.Default.</param>
		public void Sort( int index, int count, System.Collections.Generic.IComparer<T> comparer )
		{
			this.Items.Sort( index, count, comparer );
		}
		#endregion Sort

		#region Values


		#region Items
		private zeroflag.Collections.Collection<T> _Items;

		/// <summary>
		/// This collection's items.
		/// </summary>
		protected zeroflag.Collections.Collection<T> Items
		{
			get { return _Items ?? ( _Items = this.ItemsCreate ); }
			//set { _Items = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Items.
		/// This collection's items.
		/// </summary>
		protected virtual zeroflag.Collections.Collection<T> ItemsCreate
		{
			get
			{
				var items = _Items = new zeroflag.Collections.Collection<T>();
				items.ItemAdded += ( item ) =>
				{
					if ( item is T )
					{
						if ( !this.Inner.Contains( item ) )
							this.Inner.Add( item );
						this.OnItemAdded( (T)item );
					}
				};
				items.ItemRemoved += ( item ) =>
				{
					if ( item is T )
					{
						if ( this.Inner.Contains( item ) )
							this.Inner.Remove( item );
						this.OnItemRemoved( (T)item );
					}
				};
				items.ItemChanged += ( s, o, n ) =>
				{
					this.OnItemChanged( o, n );
				};
				this.Inner.ItemAdded += ( item ) =>
				{
					if ( item is T && !this.Contains( (T)item ) )
					{
						this.Add( (T)item );
					}
				};
				this.Inner.ItemRemoved += ( item ) =>
				{
					if ( item is T && this.Contains( (T)item ) )
					{
						this.Remove( (T)item );
					}
				};
				//items.ItemChanged += ( sender, olditem, newitem ) =>
				//    {
				//    };
				return items;
			}
		}

		//protected override void OnCoreChanged( Core oldvalue, Core newvalue )
		//{
		//    if ( newvalue != null )
		//    {
		//        if ( this.HasInner )
		//        {
		//            foreach ( var comp in this.Items )
		//            {
		//                comp.Core = newvalue;
		//            }
		//        }
		//    }
		//    base.OnCoreChanged( oldvalue, newvalue );
		//}

		#endregion Items

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
			this.Items.CopyTo( (T[])array, index );
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
