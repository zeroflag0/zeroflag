using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
	public class DictionaryPlus<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<TValue>//, INotifyCollectionChanged
	{
		public DictionaryPlus(params TKey[] keys)
			: this(null, keys) { }

		public DictionaryPlus(Func<TKey, TValue> defaultValue, params TKey[] keys)
			: this()
		{
			this.Default = defaultValue ?? K_Default;
			foreach (var key in keys)
				this.Add(key, this.Default(key));
		}
		protected DictionaryPlus()
		{
			this.Default = K_Default;
			this.Items = new Dictionary<TKey, TValue>();
		}

		public DictionaryPlus(Func<TValue, TKey> keyGetter, params TValue[] values)
			: this(null, keyGetter, null, values) { }

		public DictionaryPlus(Func<TKey, TValue> defaultValue, Func<TValue, TKey> keyGetter, params TValue[] values)
			: this(defaultValue, keyGetter, null, values) { }

		public DictionaryPlus(Func<TValue, TKey> keyGetter, Action<TKey, TValue> itemAdded, params TValue[] values)
			: this(null, keyGetter, itemAdded, values) { }

		public DictionaryPlus(Func<TKey, TValue> defaultValue, Func<TValue, TKey> keyGetter, Action<TKey, TValue> itemAdded, params TValue[] values)
			: this()
		{
			this.Default = defaultValue ?? K_Default;
			this.KeyGetter = keyGetter;
			if (itemAdded != null)
				this.ItemAdded += itemAdded;
			foreach (var val in values)
				this.Add(val);
		}

		static readonly Func<TKey, TValue> K_Default = k => default(TValue);
		public Func<TKey, TValue> Default { get; set; }
		public Func<TValue, TKey> KeyGetter { get; set; }
		private void Add(TKey key) { this.Add(key, this.Default(key)); }
		public void Add(TKey key, TValue value)
		{
			this[key] = value;
			this.OnItemAdded(key, value);
			//this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
		}

		protected virtual void OnItemAdded(TKey key, TValue value) { if (this.ItemAdded != null) this.ItemAdded(key, value); }
		public event Action<TKey, TValue> ItemAdded;
		//protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		//	=> this.CollectionChanged?.Invoke(this, e);
		//public event NotifyCollectionChangedEventHandler CollectionChanged;

		public TValue this[TKey key]
		{
			get
			{
				Debug.Assert(this.Default != null);
				if (!this.Items.ContainsKey(key))
					this.Items[key] = this.Default(key);
				return (this.Items)[key];
			}
			set
			{
				bool isnew = !this.Items.ContainsKey(key);
				TValue old = default(TValue);
				if (!isnew)
					old = this.Items[key];

				(this.Items)[key] = value;
				if (isnew)
				{
					this.OnItemAdded(key, value);
					//this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value));
				}
				else
				{
					//this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, old));
				}
			}
		}

		public Dictionary<TKey, TValue> Items { get; private set; }

		public ICollection<TKey> Keys { get { return (this.Items).Keys; } }

		public ICollection<TValue> Values { get { return (this.Items).Values; } }

		public int Count { get { return (this.Items).Count; } }

		public bool IsReadOnly { get { return false; } }

		public void Add(KeyValuePair<TKey, TValue> item) { this.Items.Add(item.Key, item.Value); }
		public void Clear() { (this.Items).Clear(); }
		public bool Contains(KeyValuePair<TKey, TValue> item) { return (this.Items).Contains(item); }
		public bool ContainsKey(TKey key) { return (this.Items).ContainsKey(key); }
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { this.Items.toarray().CopyTo(array, arrayIndex); }
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return (this.Items).GetEnumerator(); }
		public bool Remove(TKey key) { return (this.Items).Remove(key); }
		public bool Remove(KeyValuePair<TKey, TValue> item) { return this.Items.Remove(item.Key); }
		public bool TryGetValue(TKey key, out TValue value) { return (this.Items).TryGetValue(key, out value); }
		IEnumerator IEnumerable.GetEnumerator() { return (this.Items).GetEnumerator(); }

		public void Add(TValue item) { this.Add(this.KeyGetter(item), item); }
		public bool Contains(TValue item) { return this.ContainsKey(this.KeyGetter(item)); }
		public void CopyTo(TValue[] array, int arrayIndex) { this.Values.CopyTo(array, arrayIndex); }
		public bool Remove(TValue item) { return this.Remove(this.KeyGetter(item)); }
		IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() { return this.Values.GetEnumerator(); }
	}
}
