using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Collections
{
	public abstract class Child<ParentType, Self>
		where ParentType : Parent<Self, ParentType>
		where Self : Child<ParentType, Self>
	{
		private ParentType _Parent;

		public delegate void ParentChangedHandler(object sender, ParentType oldvalue, ParentType newvalue);

		private event ParentChangedHandler _ParentChanged;

		/// <summary>
		/// Occurs when Parent changes.
		/// </summary>
		public event ParentChangedHandler ParentChanged
		{
			add { this._ParentChanged += value; }
			remove { this._ParentChanged -= value; }
		}
		/// <summary>
		/// This node's parent node.
		/// </summary>
		public ParentType Parent
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

		/// <summary>
		/// Raises the ParentChanged event.
		/// </summary>
		protected virtual void OnParentChanged(ParentType oldvalue, ParentType newvalue)
		{
			if (oldvalue != null)
				while (oldvalue.Children.Contains((Self)this))
					oldvalue.Children.Remove((Self)this);
			if (newvalue != null && !newvalue.Children.Contains((Self)this))
				newvalue.Children.Add((Self)this);

			// if there are event subscribers...
			if (this._ParentChanged != null)
			{
				// call them...
				this._ParentChanged(this, oldvalue, newvalue);
			}
		}


		public Child()
		{
		}

		public Child(ParentType parent)
			: this()
		{
			this.Parent = parent;
		}
	}
}
