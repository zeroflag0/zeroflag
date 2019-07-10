using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Basics.Components;

namespace Basics.Components
{
	public class ValueList<TItem> : ContentList<TItem, ValueList<TItem>>
		where TItem : Content<TItem>, IValue, new()
	{
		public ValueList()
		{
		}

		public ValueList(string name)
			: base(name)
		{
		}
	}
	public class ValueList<TItem, TSelf> : ContentList<TItem, TSelf>
		where TItem : Content<TItem>, IValue, new()
		where TSelf : ValueList<TItem, TSelf>, new()
	{
		Dictionary<string, TItem> _NamedItems = new Dictionary<string, TItem>();
		protected Dictionary<string, TItem> NamedItems
		{
			get { return _NamedItems; }
		}

		Dictionary<TItem, string> _ItemNames = new Dictionary<TItem, string>();
		protected Dictionary<TItem, string> ItemNames
		{
			get { return _ItemNames; }
		}

		public TItem this[string name]
		{
			get
			{
				if (this.NamedItems.ContainsKey(name))
				{
					var item = this.NamedItems[name];
					if (item == null)
						throw new InvalidOperationException(this + " has item '" + name + "' with no value! item = " + item);
					return item;
				}
				throw new InvalidOperationException(this + " does not contain an item called '" + name + "'");
				//return this.CreateItem(name);
			}
			set
			{
				TItem target = null;
				if (value == null)
					// if no value was given, create a new parameter...
					target = this.CreateItem(name);
				else if (string.IsNullOrEmpty(value.Name) || name != value.Name)
					// if the indexed name does not match the parameter's name, create a new parameter...
					target = this.CreateItem(name);
				if (target == null)
				{
					if (this.NamedItems.ContainsKey(value.Name) && this.NamedItems[value.Name] == value)
						// if the value already is in the parameters collection we don't need to do anything, it was set by reference...
						return;
					else
						target = this.CreateItem(name);
				}
				// if a different parameter was given than was indexed, transfer the value and dismiss the given parameter (it was just a value-transfer-helper)...
				var old = this.NamedItems[name];
				if (value != null)
				{
					if (value.Name.isempty())
					{
						// if no name was given it was probably a dummyfield for setting a single value...
						if (value.Value != null)
							// if no description was given, we take the value...
							target.Value = value.Value;
						if (!value.Description.isempty())
							target.Description = value.Description;
					}
					else
					{
						target.Value = value.Value;
						target.Description = value.Description;
					}
				}
				else
				{
					target.Value = null;
					target.Description = null;
				}
				this.NamedItems[name] = target;
				//this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, target, old, this.Items.IndexOf(target)));
			}
		}

		public TItem CreateItem(string name)
		{
			lock (this.NamedItems)
				if (this.NamedItems.ContainsKey(name))
					return this.NamedItems[name];

			if (this.IsLocked)
			{
				throw new InvalidOperationException("Calculation is locked! Cannot create item '" + name + "'. (Tried to create a parameter outside of CreateFields()?)");
			}

			TItem param = new TItem() { Name = name };
			this.Add(param);

			return param;
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
			case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
				if (e.NewItems != null)
					foreach (TItem item in e.NewItems)
					{
						this.AddItem(item);
					}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				if (e.OldItems != null)
					foreach (TItem item in e.OldItems)
					{
						this.RemoveItem(item);
					}
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
				break;
			case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
				this.NamedItems.Clear();
				this.ItemNames.Clear();
				break;
			default:
				break;
			}
			base.OnCollectionChanged(e);
		}

		private void RemoveItem(TItem item)
		{
			item.PropertyChanged -= item_PropertyChanged;
			this.RefreshItem(item, null);
		}

		private void AddItem(TItem item)
		{
			item.PropertyChanged += item_PropertyChanged;
			this.RefreshItem(null, item);
		}

		void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Name")
				this.RefreshItem((TItem)sender, (TItem)sender);
		}

		private void RefreshItem(TItem old, TItem item)
		{
			lock (this.ItemNames)
			{
				lock (this.NamedItems)
				{
					string oldname = null;
					if (old != null)
					{
						if (this.ItemNames.ContainsKey(old))
						{
							oldname = this.ItemNames[old];
							this.ItemNames.Remove(old);
						}

						if (oldname != null)
						{
							this.NamedItems.Remove(oldname);
						}
						if (old.Name == null)
						{
							this.ItemNames.Remove(old);
						}
					}
					if (item != null)
					{
						this.NamedItems.Add(item.Name, item);
						this.ItemNames.Add(item, item.Name);
					}
				}
			}
		}

		public override void Dispose()
		{
			foreach (TItem item in this)
			{
				this.RemoveItem(item);
			}
			this.NamedItems.Clear();
			this.ItemNames.Clear();
			base.Dispose();
			this._NamedItems = null;
			this._ItemNames = null;
		}

		public void Add(string key, TItem value)
		{
			base.Add(value);
		}

		public bool ContainsKey(string key)
		{
			return key != null && this.NamedItems.ContainsKey(key);
		}

		public override bool Contains(TItem item)
		{
			return item != null && (this.ContainsKey(item.Name) || base.Contains(item));
		}

		public ICollection<string> Keys
		{
			get { return this.NamedItems.Keys; }
		}

		public bool Remove(string key)
		{
			if (this.ContainsKey(key))
			{
				TItem item = this[key];
				this.NamedItems.Remove(key);
				return this.Remove(item);
			}
			this.NamedItems.Remove(key);
			return false;
		}

		public bool TryGetValue(string key, out TItem value)
		{
			return this.NamedItems.TryGetValue(key, out value);
		}

		public ICollection<TItem> Values
		{
			get { return this.NamedItems.Values; }
		}

		public ValueList()
		{
		}

		public ValueList(string name)
			: base(name)
		{
		}
	}
}
