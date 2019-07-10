//#define LOGCHANGES
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace Basics.Components
{
	public class ItemList<T>
		: Savable
		, IList<T>
		//, INotifyCollectionChanged
		//, IList<T>
		//, System.Collections.ICollection
		//, System.Collections.IList
		where T : class
	{
		public ItemList(string name)
			: this()
		{
			this.Name = name;
		}

		public ItemList()
		{
			this.Items.CollectionChanged += Items_CollectionChanged;
#if LOGCHANGES
			this.Debug = true;
#endif
		}

		ObservableCollection<T> _Items = new ObservableCollection<T>();
		public ObservableCollection<T> Items
		{
			get { return _Items; }
		}

		public bool Debug { get; set; }

		private bool _IsLocked;
		/// <summary>
		/// whether changes to parameters are allowed
		/// </summary>
		public bool IsLocked
		{
			get { return _IsLocked; }
			set
			{
				if (_IsLocked != value)
				{
					_IsLocked = value;
					this.OnLockChanged(value);
					this.OnPropertyChanged("IsLocked");
				}
			}
		}

		private T _SelectedItem;
		/// <summary>
		/// the selected item of the list
		/// </summary>
		public T SelectedItem
		{
			get { return _SelectedItem; }
			set
			{
				if (_SelectedItem != value)
				{
					this.OnSelectionChanged(_SelectedItem, _SelectedItem = value);
					this.OnPropertyChanged("SelectedItem");
				}
			}
		}

		private string _Name;
		/// <summary>
		/// the collection's name
		/// </summary>
		public string Name
		{
			get { return _Name; }
			set
			{
				if (_Name != value)
				{
					_Name = value;
					this.OnPropertyChanged("Name");
				}
			}
		}


		public delegate void SelectionChangedHandler(T olditem, T newitem);
		public event SelectionChangedHandler SelectionChanged;

		protected virtual void OnSelectionChanged(T olditem, T newitem)
		{
			if (SelectionChanged != null)
				SelectionChanged(olditem, newitem);
		}



		protected virtual void OnLockChanged(bool value)
		{
		}

		public virtual void Add(T item)
		{
			if (this.IsLocked)
			{
				throw new InvalidOperationException("Calculation is locked! Cannot add item '" + item + "'. (Tried to modify parameters outside of CreateFields()?)");
			}
			if (this.Contains(item))
			{
				this.Log("Item '" + item + "' was already added!" + Environment.NewLine + new System.Diagnostics.StackTrace().ToString());
				return;
			}
			//lock (this.Items)
			{
				if (this.Contains(item))
				{
					this.Log("Item '" + item + "' was already added!" + Environment.NewLine + new System.Diagnostics.StackTrace().ToString());
					return;
				}
				this.InvokeSynchronized(() => this.Items.Add(item));
			}
			this.OnAdd(item);
		}

		public void AddRange(IEnumerable<T> items)
		{
			if (this.IsLocked)
			{
				throw new InvalidOperationException("Calculation is locked! Cannot add items. (Tried to modify parameters outside of CreateFields()?)");
			}
			foreach (T item in items)
			{
				if (this.Contains(item))
				{
					this.Log("Item '" + item + "' was already added!" + Environment.NewLine + new System.Diagnostics.StackTrace().ToString());
					return;
				}
				this.Items.Add(item);
			}
			this.OnAdd(items.ToArray());
		}

		public void Clear()
		{
			//lock (this)
			this.Clear(true);
		}

		public void Clear(bool unlock)
		{
			if (this.IsLocked && !unlock)
			{
				throw new InvalidOperationException("Calculation is locked! Cannot clear items. (Tried to modify parameters outside of CreateFields()?)");
			}
			//lock (this)
			{
				var items = this.Items.ToList();
				this.Items.Clear();
				this.OnRemove(items);
				//this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>()));
			}
		}

		public virtual bool Contains(T item)
		{
			return (item != null && this.Items.Contains(item));
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.Items.CopyTo(array, arrayIndex);
		}

		private int _InternalCount;
		/// <summary>
		/// internal count
		/// </summary>
		public int InternalCount
		{
			get { return _InternalCount; }
			set
			{
				if (_InternalCount != value)
				{
					_InternalCount = value;
					this.OnPropertyChanged("InternalCount");
					this.OnPropertyChanged("Count");
				}
			}
		}

		public int Count
		{
			get
			{
				//lock (this)
				//return InternalCount;
				return this.Items.Count;
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(T item)
		{
			if (this.IsLocked)
			{
				throw new InvalidOperationException("Calculation is locked! Cannot remove item '" + item + "'. (Tried to modify parameters outside of CreateFields()?)");
			}

			if (this.Contains(item))
			{
				lock (this)
				{
					if (this.Contains(item))
					{
						this.Items.Remove(item);
						this.OnRemove(item);
						return true;
					}
				}
			}
			return false;
		}

		public virtual IEnumerator<T> GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			//if (this.Debug)
			//	this.Log(this + ".Items: " + e.Action + " (new=" + (e.NewItems != null ? e.NewItems.Count : 0) + ", old=" + (e.OldItems != null ? e.OldItems.Count : 0) + ")");

			//switch (e.Action)
			//{
			//case NotifyCollectionChangedAction.Add:
			//	break;
			//case NotifyCollectionChangedAction.Move:
			//	break;
			//case NotifyCollectionChangedAction.Remove:
			//	break;
			//case NotifyCollectionChangedAction.Replace:
			//	break;
			//case NotifyCollectionChangedAction.Reset:
			//	this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			//	break;
			//default:
			//	break;
			//}
			//Action<Exception> exchandler = exc =>
			//	{
			//		this.Log("CollectionChanged failed (" + e.Action + ")" + Environment.NewLine + exc);
			//	};

			//this.InvokeSynchronized(() =>
			//	{
			//		if (CollectionChanged != null)
			//		{
			//			CollectionChanged(this, e);
			//		}
			//	}, exchandler);
			this.OnCollectionChanged(e);
		}

		public class FreezeContext : IDisposable
		{
			public Action Finish { get; set; }

			public void Dispose()
			{
				this.Finish();
			}
		}

		public FreezeContext Freeze()
		{
			System.Threading.Interlocked.Increment(ref _IsFrozenCounter);
			FreezeContext context = new FreezeContext();

			context.Finish = () => this.Unfreeze(context);
			return context;
		}

		public void Unfreeze(FreezeContext context)
		{
			System.Threading.Interlocked.Decrement(ref _IsFrozenCounter);
			this.Invoke(() =>
			{
				//lock (this)
				ProcessFrozenTasks();
			}, (exc) => this.Log(exc.ToString()));
			this.OnPropertyChanged("IsFrozen");
		}

		private int _IsFrozenCounter;
		/// <summary>
		/// wether this list's change notifications are being held back.
		/// </summary>
		public bool IsFrozen
		{
			get { return _IsFrozenCounter > 0; }
			//set
			//{
			//	if (_IsFrozen != value)
			//	{
			//		_IsFrozen = value;
			//		this.OnPropertyChanged("IsFrozen");
			//	}
			//}
		}

		Queue<Action> _FrozenUpdates = new Queue<Action>();
		void RefreshCount(NotifyCollectionChangedEventArgs e)
		{/*
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				for (int i = 0; i < e.NewItems.Count; i++)
					System.Threading.Interlocked.Increment(ref _InternalCount);
				this.OnPropertyChanged("Count");
			}
			else if (e.Action == NotifyCollectionChangedAction.Remove)
			{
				for (int i = 0; i < e.OldItems.Count; i++)
					if (System.Threading.Interlocked.Decrement(ref _InternalCount) <= 0)
					{
						this.Log("InternalCount[" + _InternalCount + "] is 0! Items.Count[" + Items.Count + "]");
						_InternalCount = 0;
						break;
					}
				this.OnPropertyChanged("Count");
			}
			else if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				_InternalCount = 0;
				this.OnPropertyChanged("Count");
			}
			if (_InternalCount != this.Items.Count)
			{
				this.Log("InternalCount[" + _InternalCount + "] is different than Items.Count[" + Items.Count + "]!");
			}
		  * */
		}

		int? _LastThread;
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (_LastThread != null && _LastThread != System.Threading.Thread.CurrentThread.ManagedThreadId)
			{
				try
				{
					throw new Exception("Worker thread changed from " + _LastThread + "(hash=" + this.GetHashCode() + "). "/* + Environment.NewLine + new System.Diagnostics.StackTrace()*/);
				}
				catch (Exception exc)
				{
					this.Log(exc.ToString());
				}
				finally { }
			}
			_LastThread = System.Threading.Thread.CurrentThread.ManagedThreadId;

			Action<Exception> exchandler = exc =>
				{
					this.Log("CollectionChanged failed (" + e.Action + ")" + Environment.NewLine + exc);
				};
			if (this.Debug)
				this.Log(this + ".Self-: " + e.Action + " (new=" + (e.NewItems != null ? e.NewItems.Count : 0) + ", old=" + (e.OldItems != null ? e.OldItems.Count : 0) + ", count=" + this.Count + ")");

			Action task = () =>
				{
					try
					{
						this.RefreshCount(e);
						if (CollectionChanged != null)
						{
							if (this.Debug)
								this.Log(this + ".Self+: " + e.Action + " (new=" + (e.NewItems != null ? e.NewItems.Count : 0) + ", old=" + (e.OldItems != null ? e.OldItems.Count : 0) + ", count=" + this.Count + ")");
							CollectionChanged(this, e);
						}
						if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null && e.NewItems[0].GetType().Name == "Overview")
							this.OnPropertyChanged("Count");
						else
							this.OnPropertyChanged("Count");
					}
					catch (NotSupportedException exc)
					{
						exchandler(exc);
						throw;
					}
				};
			if (this.IsFrozen)
			{
				lock (_FrozenUpdates)
					_FrozenUpdates.Enqueue(task);
			}
			else
			{
				this.Invoke(() =>
				{
					//lock (this)
					{
						ProcessFrozenTasks();
						task();
					}
				}, exchandler);
			}

		}

		private void ProcessFrozenTasks()
		{
			if (this.IsFrozen)
				return;

			if (_FrozenUpdates.Count > 0)
				lock (_FrozenUpdates)
					while (_FrozenUpdates.Count > 0)
						_FrozenUpdates.Dequeue()();
		}

		protected virtual void OnAdd(params T[] items)
		{
			//this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>(items)));
		}

		protected virtual void OnRemove(params T[] items)
		{
			foreach (T item in items)
			{
				//this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			}
			//HACK: this is not supported by .NET 4.5! this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<T>(items)));
		}

		protected virtual void OnRemove(List<T> items)
		{
			foreach (T item in items)
			{
				//this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
			}
			//HACK: this is not supported by .NET 4.5! this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
		}

		#endregion

		public override void Dispose()
		{
			lock (this)
			{
				this.Items.CollectionChanged -= Items_CollectionChanged;
				this.SelectionChanged = null;
				this.CollectionChanged = null;
				this.SelectedItem = null;
				base.Dispose();
			}
		}

		protected override void SavableFields(SavableFields fields)
		{
			throw new InvalidOperationException("This list cannot be saved!");
		}
		public Command RemoveCommand
		{
			get
			{
				return new Command(p =>
				{
					if (this.SelectedItem != null)
						this.Remove(this.SelectedItem);
				});
			}
		}

		public void CopyTo(Array array, int index)
		{
			this.Items.CopyTo((T[])array, index);
		}

		public bool IsSynchronized
		{
			get { return false; }
		}
		private object _SyncRoot = new object();
		public object SyncRoot
		{
			get { return _SyncRoot; }
		}

		public int IndexOf(T item)
		{
			return this.Items.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			if (this.Contains(item))
			{
				this.Log("Item '" + item + "' was already added!" + Environment.NewLine + new System.Diagnostics.StackTrace().ToString());
				return;
			}
			//lock (this)
			{
				if (this.Contains(item))
				{
					this.Log("Item '" + item + "' was already added!" + Environment.NewLine + new System.Diagnostics.StackTrace().ToString());
					return;
				}
				this.Items.Insert(index, item);
				this.OnAdd(item);
			}
		}

		public void RemoveAt(int index)
		{
			//lock (this)
			{
				T item = this.Items[index];
				this.Items.RemoveAt(index);
				this.OnRemove(item);
			}
		}

		public T this[int index]
		{
			get
			{
				if (this.Items.Count > index)
					return this.Items[index];
				return null;
			}
			set
			{
				if (this.Contains(value))
				{
					this.Log("Item '" + value + "' was already added!" + Environment.NewLine + new System.Diagnostics.StackTrace().ToString());
					return;
				}
				//lock (this)
				{
					if (this.Contains(value))
					{
						this.Log("Item '" + value + "' was already added!" + Environment.NewLine + new System.Diagnostics.StackTrace().ToString());
						return;
					}
					T olditem = this.Items[index];
					this.Items[index] = value;
					//this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, olditem));
				}
			}
		}

		public int Add(object value)
		{
			this.Add((T)value);
			return this.Count - 1;
		}

		public bool Contains(object value)
		{
			return value is T && this.Contains((T)value);
		}

		public int IndexOf(object value)
		{
			if (!(value is T))
				return -1;
			return this.IndexOf((T)value);
		}

		public void Insert(int index, object value)
		{
			this.Insert(index, (T)value);
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public void Remove(object value)
		{
			this.Remove((T)value);
		}

		//object System.Collections.IList.this[int index]
		//{
		//	get
		//	{
		//		return this[index];
		//	}
		//	set
		//	{
		//		this[index] = (T)value;
		//	}
		//}

		public override string ToString()
		{
			return base.ToString() + "[" + this.Count + "]" + (this.Name.isempty() ? "" : "'" + this.Name + "'");
		}

		public void PrintAll()
		{
			StringBuilder b = new StringBuilder();
			b.AppendLine("PrintAll()");
			foreach (T item in this)
			{
				b.Append("\t").Append(item).AppendLine();
			}

			this.Log(b.ToString());
		}

	}
}
