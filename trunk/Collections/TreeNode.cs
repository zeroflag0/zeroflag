using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Collections
{
	public class TreeNode<T> : IEnumerable<T>
		where T : TreeNode<T>
	{
		#region Constructors
		public TreeNode()
		{
			this.InitializeChildren();
		}

		public TreeNode(TreeNode<T> parent)
			: this()
		{
			this.Parent = parent;
		}
		#endregion Constructors


		#region Value

		public static implicit operator T(TreeNode<T> node)
		{
			return node != null ? node.Value : default(T);
		}

		public override string ToString()
		{
			return this.Value != null ? this.Value.ToString() : base.ToString();
		}

		private T m_Value;

		/// <summary>
		/// The node's value.
		/// </summary>
		public virtual T Value
		{
			get { return m_Value; }
			set
			{
				if (object.ReferenceEquals(null, m_Value) || m_Value.Equals(value))
				{
					this.OnValueChanged(m_Value, m_Value = value);
				}
			}
		}

		#region ValueChanged event
		public delegate void ValueChangedHandler(object sender, T oldvalue, T newvalue);

		private event ValueChangedHandler m_ValueChanged;
		/// <summary>
		/// Occurs when Value changes.
		/// </summary>
		public event ValueChangedHandler ValueChanged
		{
			add { this.m_ValueChanged += value; }
			remove { this.m_ValueChanged -= value; }
		}

		/// <summary>
		/// Raises the ValueChanged event.
		/// </summary>
		protected virtual void OnValueChanged(T oldvalue, T newvalue)
		{
			// if there are event subscribers...
			if (this.m_ValueChanged != null)
			{
				// call them...
				this.m_ValueChanged(this, oldvalue, newvalue);
			}
		}
		#endregion ValueChanged event
		#endregion Value

		#region Parent

		private T m_Parent;

		/// <summary>
		/// This node's parent node.
		/// </summary>
		public T Parent
		{
			get { return m_Parent; }
			set
			{
				if (m_Parent != value)
				{
					this.OnParentChanged(m_Parent, m_Parent = value);
				}
			}
		}

		#region ParentChanged event
		public delegate void ParentChangedHandler(object sender, TreeNode<T> oldvalue, TreeNode<T> newvalue);

		private event ParentChangedHandler m_ParentChanged;
		/// <summary>
		/// Occurs when Parent changes.
		/// </summary>
		public event ParentChangedHandler ParentChanged
		{
			add { this.m_ParentChanged += value; }
			remove { this.m_ParentChanged -= value; }
		}

		/// <summary>
		/// Raises the ParentChanged event.
		/// </summary>
		protected virtual void OnParentChanged(TreeNode<T> oldvalue, TreeNode<T> newvalue)
		{
			if (oldvalue != null)
				oldvalue.Remove(this);
			if (newvalue != null)
				newvalue.Add(this);

			// if there are event subscribers...
			if (this.m_ParentChanged != null)
			{
				// call them...
				this.m_ParentChanged(this, oldvalue, newvalue);
			}
		}
		#endregion ParentChanged event
		#endregion Parent

		#region System.Collections.Generic.ICollection`1

		public virtual void Add(TreeNode<T> child)
		{
			this.Children.Add(child);
		}

		public virtual void Clear()
		{
			this.Children.Clear();
		}

		public virtual bool Contains(TreeNode<T> child)
		{
			return this.Children.Contains(child);
		}

		public virtual bool Remove(TreeNode<T> child)
		{
			return this.Children.Remove(child);
		}

		public virtual int Count
		{
			get { return this.Children.Count; }
		}

		#endregion System.Collections.Generic.ICollection`1

		#region Children
		private List<TreeNode<T>> _Children = new List<TreeNode<T>>();

		public List<TreeNode<T>> Children
		{
			get { return _Children; }
		}

		private void InitializeChildren()
		{
			this.Children.ItemChanged += new List<TreeNode<T>>.ItemChangedHandler(Children_ItemChanged);
		}

		void Children_ItemChanged(object sender, TreeNode<T> oldvalue, TreeNode<T> newvalue)
		{
			if (oldvalue != null)
			{
				oldvalue.Parent = null;
			}
			if (newvalue != null)
			{
				newvalue.Parent = this;
			}
		}

		#endregion Children

		#region IEnumerable

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			//TODO: replace with tree enumerator
			return Enumerate().GetEnumerator();
		}

		public virtual System.Collections.Generic.IEnumerator<T> GetEnumerator()
		{
			//TODO: replace with tree enumerator
			return Enumerate().GetEnumerator();
		}

		System.Collections.Generic.IEnumerable<T> Enumerate()
		{
			foreach (TreeNode<T> child in Enumerate(this))
			{
				yield return child.Value;
			}
		}
		System.Collections.Generic.IEnumerable<TreeNode<T>> Enumerate(TreeNode<T> node)
		{
			foreach (TreeNode<T> child in this.Children)
			{
				yield return child;
				foreach (TreeNode<T> inner in Enumerate(child))
				{
					yield return inner;
				}
			}
		}

		#endregion IEnumerable

	}
}
