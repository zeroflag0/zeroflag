using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Collections
{
	public class CollectionSynchronizer<ItemType, PeerType>
	{
		public CollectionSynchronizer(ICollection<ItemType> source, PeerCreationHandler create, PeerRemoveHandler remove, PeerUpdateHandler update)
		{
			this.Items = source;
			this.PeerCreator = create;
			this.PeerRemover = remove;
			this.PeerUpdater = update;
			Console.WriteLine("CollectionSynchronizer<" + typeof(ItemType).Name + ", " + typeof(PeerType).Name + "> created:\n" + new System.Diagnostics.StackTrace());
		}

		#region Collections
		ICollection<ItemType> _Items;

		public ICollection<ItemType> Items
		{
			get { return _Items; }
			set { _Items = value; }
		}

		Dictionary<ItemType, PeerType> _ItemPeers = new Dictionary<ItemType, PeerType>();

		public Dictionary<ItemType, PeerType> ItemPeers
		{
			get { return _ItemPeers; }
		}

		Dictionary<PeerType, ItemType> _PeerItems = new Dictionary<PeerType, ItemType>();

		public Dictionary<PeerType, ItemType> PeerItems
		{
			get { return _PeerItems; }
		}
		#endregion Collections

		public virtual void Synchronize()
		{
			List<ItemType> items = new List<ItemType>(this.Items);
			List<ItemType> removes = new List<ItemType>();

			foreach (ItemType item in items)
			{
				this.SynchronizeItem(item);
			}

			foreach (PeerType peer in this.PeerItems.Keys)
			{
				if (!items.Contains(this.PeerItems[peer]))
					removes.Add(this.PeerItems[peer]);
			}

			foreach (ItemType item in removes)
			{
				PeerType peer = this.ItemPeers[item];

				this.ItemPeers.Remove(item);
				this.PeerItems.Remove(peer);
				if (this.PeerRemover != null)
					this.PeerRemover(peer);
				Console.WriteLine(this + " Removed peer " + peer + "(" + peer.GetHashCode() + ")" + " for item " + item + "(" + item.GetHashCode() + ")");
			}
		}

		protected virtual void SynchronizeItem(ItemType item)
		{
			PeerType peer;
			if (!this.ItemPeers.ContainsKey(item))
			{
				// create peer...
				//Console.WriteLine("Creating peer for item " + item + "(" + item.GetHashCode() + ")");
				peer = this.PeerCreator(item);

				this.ItemPeers.Add(item, peer);
				this.PeerItems.Add(peer, item);
				Console.WriteLine(this + " Created peer " + peer + "(" + peer.GetHashCode() + ")" + " for item " + item + "(" + item.GetHashCode() + ")");
			}
			else
				peer = this.ItemPeers[item];

			if (this.PeerUpdater != null)
				this.PeerUpdater(item, peer);
			Console.WriteLine(this + " Updated peer " + peer + "(" + peer.GetHashCode() + ")" + " for item " + item + "(" + item.GetHashCode() + ")");
		}

		#region PeerUpdater
		public delegate void PeerUpdateHandler(ItemType item, PeerType peer);

		private PeerUpdateHandler _PeerUpdater = default(PeerUpdateHandler);

		public PeerUpdateHandler PeerUpdater
		{
			get { return _PeerUpdater; }
			set
			{
				if (_PeerUpdater != value)
				{
					_PeerUpdater = value;
				}
			}
		}
		#endregion PeerUpdater

		#region PeerRemover
		public delegate void PeerRemoveHandler(PeerType peer);

		private PeerRemoveHandler _PeerRemover = default(PeerRemoveHandler);

		public PeerRemoveHandler PeerRemover
		{
			get { return _PeerRemover; }
			set
			{
				if (_PeerRemover != value)
				{
					_PeerRemover = value;
				}
			}
		}
		#endregion PeerRemover

		#region PeerCreator
		public delegate PeerType PeerCreationHandler(ItemType item);

		private PeerCreationHandler _PeerCreator = default(PeerCreationHandler);

		public PeerCreationHandler PeerCreator
		{
			get { return _PeerCreator; }
			set
			{
				if (_PeerCreator != value)
				{
					_PeerCreator = value;
				}
			}
		}
		#endregion PeerCreator

		//#region ItemCreator
		//public delegate ItemType ItemCreationHandler(ItemType item);

		//private ItemCreationHandler _ItemCreator = default(ItemCreationHandler);

		//public ItemCreationHandler ItemCreator
		//{
		//    get { return _ItemCreator; }
		//    set
		//    {
		//        if (_ItemCreator != value)
		//        {
		//            _ItemCreator = value;
		//        }
		//    }
		//}
		//#endregion ItemCreator

		public override string ToString()
		{
			return this.GetType().Name + " " + this.GetHashCode();
		}
	}
}
