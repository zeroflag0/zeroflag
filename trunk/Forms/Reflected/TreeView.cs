using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace zeroflag.Forms.Reflected
{
	public partial class TreeView<T> : UserControl
		where T : class
	{
		public TreeView()
		{
			InitializeComponent();

			this.Control.NodeMouseClick += new TreeNodeMouseClickEventHandler( Control_NodeMouseClick );
		}

		void Control_NodeMouseClick( object sender, TreeNodeMouseClickEventArgs e )
		{
			this.SynchronizeSelected();
		}

		[Browsable( false )]
		public System.Windows.Forms.TreeView Control
		{
			get { return _Control; }
		}

		#region System.Windows.Forms.TreeView

		public System.Windows.Forms.BorderStyle BorderStyle
		{
			get { return this.Control.BorderStyle; }
			set { this.Control.BorderStyle = value; }
		}

		public bool CheckBoxes
		{
			get { return this.Control.CheckBoxes; }
			set { this.Control.CheckBoxes = value; }
		}

		public bool FullRowSelect
		{
			get { return this.Control.FullRowSelect; }
			set { this.Control.FullRowSelect = value; }
		}

		public bool HideSelection
		{
			get { return this.Control.HideSelection; }
			set { this.Control.HideSelection = value; }
		}

		public bool HotTracking
		{
			get { return this.Control.HotTracking; }
			set { this.Control.HotTracking = value; }
		}

		public int ImageIndex
		{
			get { return this.Control.ImageIndex; }
			set { this.Control.ImageIndex = value; }
		}

		public System.String ImageKey
		{
			get { return this.Control.ImageKey; }
			set { this.Control.ImageKey = value; }
		}

		public System.Windows.Forms.ImageList ImageList
		{
			get { return this.Control.ImageList; }
			set { this.Control.ImageList = value; }
		}

		public System.Windows.Forms.ImageList StateImageList
		{
			get { return this.Control.StateImageList; }
			set { this.Control.StateImageList = value; }
		}

		public int Indent
		{
			get { return this.Control.Indent; }
			set { this.Control.Indent = value; }
		}

		public int ItemHeight
		{
			get { return this.Control.ItemHeight; }
			set { this.Control.ItemHeight = value; }
		}

		public bool LabelEdit
		{
			get { return this.Control.LabelEdit; }
			set { this.Control.LabelEdit = value; }
		}

		public System.Drawing.Color LineColor
		{
			get { return this.Control.LineColor; }
			set { this.Control.LineColor = value; }
		}

		public System.Windows.Forms.TreeNodeCollection Nodes
		{
			get { return this.Control.Nodes; }
		}

		public System.Windows.Forms.TreeViewDrawMode DrawMode
		{
			get { return this.Control.DrawMode; }
			set { this.Control.DrawMode = value; }
		}

		public System.String PathSeparator
		{
			get { return this.Control.PathSeparator; }
			set { this.Control.PathSeparator = value; }
		}

		public virtual bool RightToLeftLayout
		{
			get { return this.Control.RightToLeftLayout; }
			set { this.Control.RightToLeftLayout = value; }
		}

		public bool Scrollable
		{
			get { return this.Control.Scrollable; }
			set { this.Control.Scrollable = value; }
		}

		public int SelectedImageIndex
		{
			get { return this.Control.SelectedImageIndex; }
			set { this.Control.SelectedImageIndex = value; }
		}

		public System.String SelectedImageKey
		{
			get { return this.Control.SelectedImageKey; }
			set { this.Control.SelectedImageKey = value; }
		}

		public TreeViewItem<T> SelectedNode
		{
			get { return this.Control.SelectedNode as TreeViewItem<T>; }
			set { this.Control.SelectedNode = value; }
		}

		public bool ShowLines
		{
			get { return this.Control.ShowLines; }
			set { this.Control.ShowLines = value; }
		}

		public bool ShowNodeToolTips
		{
			get { return this.Control.ShowNodeToolTips; }
			set { this.Control.ShowNodeToolTips = value; }
		}

		public bool ShowPlusMinus
		{
			get { return this.Control.ShowPlusMinus; }
			set { this.Control.ShowPlusMinus = value; }
		}

		public bool ShowRootLines
		{
			get { return this.Control.ShowRootLines; }
			set { this.Control.ShowRootLines = value; }
		}

		public bool Sorted
		{
			get { return this.Control.Sorted; }
			set { this.Control.Sorted = value; }
		}

		public System.Collections.IComparer TreeViewNodeSorter
		{
			get { return this.Control.TreeViewNodeSorter; }
			set { this.Control.TreeViewNodeSorter = value; }
		}

		public System.Windows.Forms.TreeNode TopNode
		{
			get { return this.Control.TopNode; }
			set { this.Control.TopNode = value; }
		}

		public int VisibleCount
		{
			get { return this.Control.VisibleCount; }
		}

		public event System.Windows.Forms.NodeLabelEditEventHandler BeforeLabelEdit
		{
			add { this.Control.BeforeLabelEdit += value; }
			remove { this.Control.BeforeLabelEdit -= value; }
		}

		public event System.Windows.Forms.NodeLabelEditEventHandler AfterLabelEdit
		{
			add { this.Control.AfterLabelEdit += value; }
			remove { this.Control.AfterLabelEdit -= value; }
		}

		public event System.Windows.Forms.TreeViewCancelEventHandler BeforeCheck
		{
			add { this.Control.BeforeCheck += value; }
			remove { this.Control.BeforeCheck -= value; }
		}

		public event System.Windows.Forms.TreeViewEventHandler AfterCheck
		{
			add { this.Control.AfterCheck += value; }
			remove { this.Control.AfterCheck -= value; }
		}

		public event System.Windows.Forms.TreeViewCancelEventHandler BeforeCollapse
		{
			add { this.Control.BeforeCollapse += value; }
			remove { this.Control.BeforeCollapse -= value; }
		}

		public event System.Windows.Forms.TreeViewEventHandler AfterCollapse
		{
			add { this.Control.AfterCollapse += value; }
			remove { this.Control.AfterCollapse -= value; }
		}

		public event System.Windows.Forms.TreeViewCancelEventHandler BeforeExpand
		{
			add { this.Control.BeforeExpand += value; }
			remove { this.Control.BeforeExpand -= value; }
		}

		public event System.Windows.Forms.TreeViewEventHandler AfterExpand
		{
			add { this.Control.AfterExpand += value; }
			remove { this.Control.AfterExpand -= value; }
		}

		public event System.Windows.Forms.DrawTreeNodeEventHandler DrawNode
		{
			add { this.Control.DrawNode += value; }
			remove { this.Control.DrawNode -= value; }
		}

		public event System.Windows.Forms.ItemDragEventHandler ItemDrag
		{
			add { this.Control.ItemDrag += value; }
			remove { this.Control.ItemDrag -= value; }
		}

		public event System.Windows.Forms.TreeNodeMouseHoverEventHandler NodeMouseHover
		{
			add { this.Control.NodeMouseHover += value; }
			remove { this.Control.NodeMouseHover -= value; }
		}

		public event System.Windows.Forms.TreeViewCancelEventHandler BeforeSelect
		{
			add { this.Control.BeforeSelect += value; }
			remove { this.Control.BeforeSelect -= value; }
		}

		public event System.Windows.Forms.TreeViewEventHandler AfterSelect
		{
			add { this.Control.AfterSelect += value; }
			remove { this.Control.AfterSelect -= value; }
		}

		public event System.Windows.Forms.TreeNodeMouseClickEventHandler NodeMouseClick
		{
			add { this.Control.NodeMouseClick += value; }
			remove { this.Control.NodeMouseClick -= value; }
		}

		public event System.Windows.Forms.TreeNodeMouseClickEventHandler NodeMouseDoubleClick
		{
			add { this.Control.NodeMouseDoubleClick += value; }
			remove { this.Control.NodeMouseDoubleClick -= value; }
		}

		public event System.EventHandler RightToLeftLayoutChanged
		{
			add { this.Control.RightToLeftLayoutChanged += value; }
			remove { this.Control.RightToLeftLayoutChanged -= value; }
		}

		public void BeginUpdate()
		{
			this.Control.BeginUpdate();
		}

		public void CollapseAll()
		{
			this.Control.CollapseAll();
		}

		public void EndUpdate()
		{
			this.Control.EndUpdate();
		}

		public void ExpandAll()
		{
			this.Control.ExpandAll();
		}

		public System.Windows.Forms.TreeViewHitTestInfo HitTest( System.Drawing.Point pt )
		{
			return this.Control.HitTest( pt );
		}

		public int GetNodeCount( bool includeSubTrees )
		{
			return this.Control.GetNodeCount( includeSubTrees );
		}

		public System.Windows.Forms.TreeNode GetNodeAt( System.Drawing.Point pt )
		{
			return this.Control.GetNodeAt( pt );
		}

		public void Sort()
		{
			this.Control.Sort();
		}

		#endregion System.Windows.Forms.TreeView


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

		public delegate void ModifyChildHandler( T parent, T item );

		#region AddChildCallback
		private ModifyChildHandler _AddChildCallback;

		/// <summary>
		/// Callback used to add a child to a parent instance.
		/// </summary>
		public ModifyChildHandler AddChildCallback
		{
			get { return _AddChildCallback ?? ( _AddChildCallback = this.AddChildCallbackCreate ); }
			set { _AddChildCallback = value; }
		}

		/// <summary>
		/// Creates the default/initial value for AddChildCallback.
		/// Callback used to add a child to a parent instance.
		/// </summary>
		protected virtual ModifyChildHandler AddChildCallbackCreate
		{
			get
			{
				Type children = null;
				ModifyChildHandler handler = null;
				if ( typeof( ICollection<T> ).IsAssignableFrom( typeof( T ) ) )
				{
					children = typeof( T );
					var method = children.GetMethod( "Add", new Type[] { typeof( T ) } );

					handler = ( parent, child ) =>
					{
						method.Invoke( parent, new object[] { child } );
					};
				}
				else
				{
					// search the type's members for possible child collections...
					foreach ( var member in typeof( T ).GetProperties() )
					{
						if ( typeof( ICollection<T> ).IsAssignableFrom( member.PropertyType ) )
						{
							children = member.PropertyType;
							var getMethod = member.GetGetMethod();
							var method = children.GetMethod( "Add", new Type[] { typeof( T ) } );
							handler = ( parent, child ) =>
							{
								method.Invoke( getMethod.Invoke( parent, null ), new object[] { child } );
							};
							break;
						}
					}
				}
				return handler;
			}
		}

		#endregion AddChildCallback

		#region RemoveChildCallback
		private ModifyChildHandler _RemoveChildCallback;

		/// <summary>
		/// Callback used to remove a child from a parent instance.
		/// </summary>
		public ModifyChildHandler RemoveChildCallback
		{
			get { return _RemoveChildCallback ?? ( _RemoveChildCallback = this.RemoveChildCallbackCreate ); }
			set { _RemoveChildCallback = value; }
		}

		/// <summary>
		/// Creates the default/initial value for RemoveChildCallback.
		/// Callback used to remove a child from a parent instance.
		/// </summary>
		protected virtual ModifyChildHandler RemoveChildCallbackCreate
		{
			get
			{
				Type children = null;
				ModifyChildHandler handler = null;
				if ( typeof( ICollection<T> ).IsAssignableFrom( typeof( T ) ) )
				{
					children = typeof( T );
					var method = children.GetMethod( "Remove", new Type[] { typeof( T ) } );

					handler = ( parent, child ) =>
					{
						method.Invoke( parent, new object[] { child } );
					};
				}
				else
				{
					// search the type's members for possible child collections...
					foreach ( var member in typeof( T ).GetProperties() )
					{
						if ( typeof( ICollection<T> ).IsAssignableFrom( member.PropertyType ) )
						{
							children = member.PropertyType;
							var getMethod = member.GetGetMethod();
							var method = children.GetMethod( "Remove", new Type[] { typeof( T ) } );
							handler = ( parent, child ) =>
							{
								method.Invoke( getMethod.Invoke( parent, null ), new object[] { child } );
							};
							break;
						}
					}
				}
				return handler;
			}
		}

		#endregion RemoveChildCallback

		public delegate IEnumerable<T> GetChildEnumeratorHandler( T item );


		#region GetChildEnumeratorCallback
		private GetChildEnumeratorHandler _GetChildEnumeratorCallback;

		/// <summary>
		/// Callback used to retrieve a child-enumerator for a given instance.
		/// </summary>
		public GetChildEnumeratorHandler GetChildEnumeratorCallback
		{
			get { return _GetChildEnumeratorCallback ?? ( _GetChildEnumeratorCallback = this.GetChildEnumeratorCallbackCreate ); }
			set { _GetChildEnumeratorCallback = value; }
		}

		/// <summary>
		/// Creates the default/initial value for GetChildEnumeratorCallback.
		/// Callback used to retrieve a child-enumerator for a given instance.
		/// </summary>
		protected virtual GetChildEnumeratorHandler GetChildEnumeratorCallbackCreate
		{
			get
			{
				Type children = null;
				GetChildEnumeratorHandler handler = null;
				if ( typeof( IEnumerable<T> ).IsAssignableFrom( typeof( T ) ) )
				{
					children = typeof( T );
					var method = children.GetMethod( "GetEnumerator", new Type[] { } );

					handler = item =>
					{
						return (IEnumerable<T>)item;
					};
				}
				else
				{
					// search the type's members for possible child collections...
					foreach ( var member in typeof( T ).GetProperties() )
					{
						if ( typeof( ICollection<T> ).IsAssignableFrom( member.PropertyType ) )
						{
							children = member.PropertyType;
							var getMethod = member.GetGetMethod();
							var method = children.GetMethod( "GetEnumerator", new Type[] { } );

							handler = item =>
							{
								return (IEnumerable<T>)getMethod.Invoke( item, null );
							};
							break;
						}
					}
				}
				return handler;

			}
		}

		#endregion GetChildEnumeratorCallback


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
			this.SynchronizeSelected();
		}

		public virtual TreeViewItem<T> Synchronize( T item, T parent )
		{
			if ( item == null )
				return null;
			//Console.WriteLine( "Synchronize(" + item + ", " + ( ( (object)parent ) ?? "<null>" ) + ")" );
			TreeViewItem<T> outer = null;
			TreeViewItem<T> view;

			if ( parent != null )
			{
				outer = this.ItemPeers[ parent ];
				//outer = this.Synchronize( parent, null );
			}

			if ( !this.ItemPeers.ContainsKey( item ) )
			{
				// create peer...
				view = new TreeViewItem<T>( item ) { Owner = this };

				this.ItemPeers.Add( item, view );
				this.PeerItems.Add( view, item );

				if ( outer != null )
					outer.Nodes.Add( view );
				else
					this.Nodes.Add( view );
			}
			else
				view = this.ItemPeers[ item ];

			view.Update();

			List<T> children = new List<T>();
			foreach ( T child in this.GetChildEnumeratorCallback( item ) )
			{
				this.Synchronize( child, item );
				children.Add( child );
			}

			if ( view.Nodes.Count != children.Count )
			{
				List<TreeViewItem<T>> views = new List<TreeViewItem<T>>();
				foreach ( TreeViewItem<T> child in view.Nodes )
					views.Add( child );

				foreach ( TreeViewItem<T> child in views )
				{
					if ( !children.Contains( child.Value ) )
						view.Nodes.Remove( child );
				}
			}

			return view;
		}

		public void SynchronizeSelected()
		{
			if ( this.SelectedNode != null )
			{
				this.SelectedItem = this.PeerItems[ this.SelectedNode as TreeViewItem<T> ];
				this.SelectedNode.Expand();
			}
			else
			{
				this.SelectedItem = null;
			}
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
