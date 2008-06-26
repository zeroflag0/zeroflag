using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace zeroflag.Forms.Reflected
{
	//public class T { }

	public partial class TreeView<T> : UserControl
		where T : class
	{
		public TreeView()
		{
			InitializeComponent();
		}

		[Browsable( false )]
		public System.Windows.Forms.TreeView Control
		{
			get { return _Control; }
		}


		#region Node

		private T _Node;

		/// <summary>
		/// The root node shown in the TreeView.
		/// </summary>
		public T Node
		{
			get { return _Node; }
			set
			{
				if ( _Node != value )
				{
					this.OnNodeChanged( _Node, _Node = value );
				}
			}
		}

		#region NodeChanged event
		public delegate void NodeChangedHandler( object sender, T oldvalue, T newvalue );

		private event NodeChangedHandler _NodeChanged;
		/// <summary>
		/// Occurs when Node changes.
		/// </summary>
		public event NodeChangedHandler NodeChanged
		{
			add { this._NodeChanged += value; }
			remove { this._NodeChanged -= value; }
		}

		/// <summary>
		/// Raises the NodeChanged event.
		/// </summary>
		protected virtual void OnNodeChanged( T oldvalue, T newvalue )
		{
			// if there are event subscribers...
			if ( this._NodeChanged != null )
			{
				// call them...
				this._NodeChanged( this, oldvalue, newvalue );
			}
		}
		#endregion NodeChanged event
		#endregion Node


		#region SelectedItem

		private T _SelectedItem;

		/// <summary>
		/// The currently selected item/node.
		/// </summary>
		public T SelectedItem
		{
			get { return _SelectedItem; }
			set
			{
				if ( _SelectedItem != value )
				{
					this.OnSelectedItemChanged( _SelectedItem, _SelectedItem = value );
				}
			}
		}

		#region SelectedItemChanged event
		public delegate void SelectedItemChangedHandler( object sender, T oldvalue, T newvalue );

		private event SelectedItemChangedHandler _SelectedItemChanged;
		/// <summary>
		/// Occurs when SelectedItem changes.
		/// </summary>
		public event SelectedItemChangedHandler SelectedItemChanged
		{
			add { this._SelectedItemChanged += value; }
			remove { this._SelectedItemChanged -= value; }
		}

		/// <summary>
		/// Raises the SelectedItemChanged event.
		/// </summary>
		protected virtual void OnSelectedItemChanged( T oldvalue, T newvalue )
		{
			// if there are event subscribers...
			if ( this._SelectedItemChanged != null )
			{
				// call them...
				this._SelectedItemChanged( this, oldvalue, newvalue );
			}
		}
		#endregion SelectedItemChanged event
		#endregion SelectedItem

		#region Synchronization

		Dictionary<T, TreeViewItem<T>> _ItemPeers = new Dictionary<T, TreeViewItem<T>>();

		public Dictionary<T, TreeViewItem<T>> ItemPeers
		{
			get { return _ItemPeers; }
		}

		Dictionary<TreeViewItem<T>, T> _PeerItems = new Dictionary<TreeViewItem<T>, T>();

		public Dictionary<TreeViewItem<T>, T> PeerItems
		{
			get { return _PeerItems; }
		}


		public virtual void Synchronize()
		{
			this.Synchronize( this.Node, null );
		}

		public virtual TreeViewItem<T> Synchronize( T item, T parent )
		{
			if ( item == null )
				return null;
			TreeViewItem<T> outer = null;
			TreeViewItem<T> view;
			if ( !this.ItemPeers.ContainsKey( item ) )
			{
				// create peer...
				view = new TreeViewItem<T>( item );

				this.ItemPeers.Add( item, view );
				this.PeerItems.Add( view, item );
			}
			else
				view = this.ItemPeers[ item ];

			if ( parent != null )
			{
				outer = this.Synchronize( parent, null );
			}


			return view;
		}

		#endregion Synchronization

		#region AutoSynchronize
		public bool AutoSynchronize
		{
			get { return this.timer.Enabled; }
			set
			{
				if ( this.timer.Enabled != value )
				{
					this.timer.Enabled = value;
				}
			}
		}

		#region AutoSynchronizeInterval
		public int AutoSynchronizeInterval
		{
			get { return this.timer.Interval; }
			set
			{
				if ( this.timer.Interval != value )
				{
					this.timer.Interval = value;
				}
			}
		}
		#endregion AutoSynchronizeInterval

		#endregion AutoSynchronize


		protected override void OnInvalidated( InvalidateEventArgs e )
		{
			this.Synchronize();
			base.OnInvalidated( e );
		}

		protected override void OnVisibleChanged( EventArgs e )
		{
			base.OnVisibleChanged( e );
			if ( this.Visible )
				this.Synchronize();
		}

		protected override void OnLoad( EventArgs e )
		{
			base.OnLoad( e );
			this.Synchronize();
		}

	}
}
