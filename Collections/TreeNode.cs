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
	public abstract class TreeNode<TSelf>
		: zeroflag.Components.Component
		  , ITreeNode<TSelf>
		where TSelf : class, ITreeNode<TSelf>
	{
		#region Constructors

		public TreeNode()
		{
			this.InitializeChildren();
		}

		public TreeNode(TSelf parent)
			: this()
		{
			this.Parent = parent;
		}

		#endregion Constructors

		#region Value

#if TREENODE_USEVALUE
		public static implicit operator T(TreeNode<T> node)
		{
			return node != null ? node.Value : default(T);
		}

		public override string ToString()
		{
			return this.Value != null ? this.Value.ToString() : base.ToString();
		}

		private T _Value;

		/// <summary>
		/// The node's value.
		/// </summary>
		public virtual T Value
		{
			get { return _Value; }
			set
			{
				if (object.ReferenceEquals(null, _Value) || _Value.Equals(value))
				{
					this.OnValueChanged(_Value, _Value = value);
				}
			}
		}

		#region ValueChanged event
		public delegate void ValueChangedHandler(object sender, T oldvalue, T newvalue);

		private event ValueChangedHandler _ValueChanged;
		/// <summary>
		/// Occurs when Value changes.
		/// </summary>
		public event ValueChangedHandler ValueChanged
		{
			add { this._ValueChanged += value; }
			remove { this._ValueChanged -= value; }
		}

		/// <summary>
		/// Raises the ValueChanged event.
		/// </summary>
		protected virtual void OnValueChanged(T oldvalue, T newvalue)
		{
			// if there are event subscribers...
			if (this._ValueChanged != null)
			{
				// call them...
				this._ValueChanged(this, oldvalue, newvalue);
			}
		}
		#endregion ValueChanged event
#endif

		#endregion Value

		#region Parent

		private TSelf _Parent;

		/// <summary>
		/// This node's parent node.
		/// </summary>
		public TSelf Parent
		{
			get { return _Parent; }
			set
			{
				if (_Parent != value)
				{
					this.OnParentChanged(_Parent, _Parent = value);
				}
			}
		}

		#region ParentChanged event

		private event Action<object, TSelf, TSelf> _ParentChanged;

		/// <summary>
		/// Occurs when Parent changes.
		/// </summary>
		public event Action<object, TSelf, TSelf> ParentChanged
		{
			add { this._ParentChanged += value; }
			remove { this._ParentChanged -= value; }
		}

		/// <summary>
		/// Raises the ParentChanged event.
		/// </summary>
		protected virtual void OnParentChanged(TSelf oldvalue, TSelf newvalue)
		{
			if (oldvalue != null)
			{
				while (oldvalue.Contains((TSelf)(ITreeNode<TSelf>) this))
				{
					oldvalue.Remove((TSelf)(ITreeNode<TSelf>)this);
				}
			}
			if (newvalue != null && !newvalue.Contains((TSelf)(ITreeNode<TSelf>)this))
			{
				newvalue.Add((TSelf)(ITreeNode<TSelf>)this);
			}

			// if there are event subscribers...
			if (this._ParentChanged != null)
			{
				// call them...
				this._ParentChanged(this, oldvalue, newvalue);
			}
		}

		#endregion ParentChanged event

		#endregion Parent

		#region System.Collections.Generic.ICollection`1

		public virtual void Add(TSelf child)
		{
			this.Children.Add(child);
		}

		public virtual void Clear()
		{
			this.Children.Clear();
		}

		public virtual bool Contains(TSelf child)
		{
			return this.Children.Contains(child);
		}

		public virtual bool Remove(TSelf child)
		{
			return this.Children.Remove(child);
		}

		public virtual int Count
		{
			get { return this.Children.Count; }
		}

		#endregion System.Collections.Generic.ICollection`1

		#region Children

		#region Children

		private List<TSelf> _Children;

		/// <summary>
		/// Child nodes.
		/// </summary>
		[System.ComponentModel.Browsable(false)]
		public List<TSelf> Children
		{
			get { return _Children ?? (_Children = this.ChildrenCreate); }
			//set { _Children = value; }
		}

		/// <summary>
		/// Creates the default/initial value for Children.
		/// Child nodes.
		/// </summary>
		protected virtual List<TSelf> ChildrenCreate
		{
			get
			{
				var children = _Children = new List<TSelf>();
				this.InitializeChildren();
				return children;
			}
		}

		#endregion Children

		//private List<Self> _Children = new List<Self>();
		//[System.ComponentModel.Browsable(false)]
		//public List<Self> Children
		//{
		//    get { return _Children; }
		//    set
		//    {
		//        if (_Children != value && value != null)
		//        {
		//            this._Children.Clear();
		//            foreach (Self item in value)
		//            {
		//                this._Children.Add(item);
		//            }
		//            //_Children = value;
		//        }
		//    }
		//}

		private void InitializeChildren()
		{
			this.Children.ItemChanged += this.Children_ItemChanged;
		}

		private void Children_ItemChanged(object sender, ITreeNode<TSelf> oldvalue, ITreeNode<TSelf> newvalue)
		{
			if (oldvalue != null)
			{
				oldvalue.Parent = null;
			}
			if (newvalue != null)
			{
				newvalue.Parent = (TSelf)(ITreeNode<TSelf>)this;
			}
		}

		#endregion Children

		#region IEnumerable

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return Enumerate().GetEnumerator();
		}

		public virtual System.Collections.Generic.IEnumerator<TSelf> GetEnumerator()
		{
			return Enumerate().GetEnumerator();
		}

		public System.Collections.Generic.IEnumerable<TSelf> Enumerate()
		{
			foreach (TSelf child in this.Children)
			{
				yield return child;
				foreach (TSelf inner in child.Enumerate())
				{
					yield return inner;
				}
			}
		}

		//System.Collections.Generic.IEnumerable<TreeNode<Self>> EnumerateT()
		//{
		//    foreach ( TreeNode<Self> child in this.Children )
		//    {
		//        yield return child;
		//        foreach ( TreeNode<Self> inner in child.EnumerateT() )
		//        {
		//            yield return inner;
		//        }
		//    }
		//}

		#endregion IEnumerable
	}
}