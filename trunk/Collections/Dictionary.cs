using System;

namespace zeroflag.Collections
{
	public class Dictionary<TKey, TValue> : System.Collections.Generic.IDictionary<TKey, TValue>, System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>
	{

		#region Items
		private System.Collections.Generic.Dictionary<TKey, TValue> _Items;

		/// <summary>
		/// The actual collection implementation...
		/// </summary>
		public System.Collections.Generic.Dictionary<TKey, TValue> Items
		{
			get { return _Items ?? ( _Items = this.ItemsCreate ); }
			//set { _Items = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Items.
		/// The actual collection implementation...
		/// </summary>
		protected virtual System.Collections.Generic.Dictionary<TKey, TValue> ItemsCreate
		{
			get
			{
				var Items = _Items = new System.Collections.Generic.Dictionary<TKey, TValue>();
				return Items;
			}
		}

		#endregion Items

		#region DefaultValue
		public delegate TValue CreateDefaultValueHandler( TKey key );

		private CreateDefaultValueHandler _DefaultValue;

		/// <summary>
		/// Create a default value if the list doesn't have one.
		/// </summary>
		public CreateDefaultValueHandler DefaultValue
		{
			get { return _DefaultValue ?? ( _DefaultValue = this.DefaultValueCreate ); }
			set { _DefaultValue = value; }
		}

		/// <summary>
		/// Creates the default/initial value for DefaultValue.
		/// Create a default value if the list doesn't have one.
		/// </summary>
		protected virtual CreateDefaultValueHandler DefaultValueCreate
		{
			get
			{
				var DefaultValue = _DefaultValue = key => default( TValue );
				return DefaultValue;
			}
		}

		#endregion DefaultValue

		#region IDictionary<TKey,TValue> Members

		public void Add( TKey key, TValue value )
		{
			this.Items.Add( key, value );
		}

		public System.Collections.Generic.ICollection<TKey> Keys
		{
			get { return this.Items.Keys; }
		}

		public bool Remove( TKey key )
		{
			return this.Items.Remove( key );
		}

		public bool TryGetValue( TKey key, out TValue value )
		{
			return this.Items.TryGetValue( key, out value );
		}

		public System.Collections.Generic.ICollection<TValue> Values
		{
			get { return this.Items.Values; }
		}

		public TValue this[TKey key]
		{
			get
			{
				if ( this.Items.ContainsKey( key ) )
					return this.Items[key];
				else
					return this.Items[key] = this.DefaultValue( key );
			}
			set
			{
				this.Items[key] = value;
			}
		}


		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		public void Clear()
		{
			this.Items.Clear();
		}

		public int Count
		{
			get { return this.Items.Count; }
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>> Members

		void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Add( System.Collections.Generic.KeyValuePair<TKey, TValue> item )
		{
			( (System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)this.Items ).Add( item );
		}

		bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Contains( System.Collections.Generic.KeyValuePair<TKey, TValue> item )
		{
			return ( (System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)this.Items ).Contains( item );
		}

		void System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.CopyTo( System.Collections.Generic.KeyValuePair<TKey, TValue>[] array, int arrayIndex )
		{
			( (System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)this.Items ).CopyTo( array, arrayIndex );
		}

		bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.Remove( System.Collections.Generic.KeyValuePair<TKey, TValue> item )
		{
			return ( (System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)this.Items ).Remove( item );
		}

		bool System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get { return ( (System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>)this.Items ).IsReadOnly; }
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>> Members

		public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<TKey, TValue>> GetEnumerator()
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

		#region Advanced
		public bool ContainsValue( TValue value )
		{
			return this.Items.ContainsValue( value );
		}

		public bool ContainsKey( TKey key )
		{
			return this.Items.ContainsKey( key );
		}
		#endregion
	}
}
