using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Collections
{
	public abstract class Parent<ChildType, Self> : IEnumerable<ChildType>
		where ChildType : Child<Self, ChildType>
		where Self : Parent<ChildType, Self>
	{
		private List<ChildType> _Children = new List<ChildType>();
		[System.ComponentModel.Browsable(false)]
		public List<ChildType> Children
		{
			get { return _Children; }
			set
			{
				if (_Children != value && value != null)
				{
					this._Children.Clear();
					foreach (ChildType item in value)
					{
						this._Children.Add(item);
					}
					//_Children = value;
				}
			}
		}

		void Children_ItemChanged(object sender, ChildType oldvalue, ChildType newvalue)
		{
			if (oldvalue != null)
			{
				oldvalue.Parent = null;
			}
			if (newvalue != null)
			{
				newvalue.Parent = (Self)this;
			}
		}

		private void InitializeChildren()
		{
			this.Children.ItemChanged += this.Children_ItemChanged;
		}

		public virtual System.Collections.Generic.IEnumerator<ChildType> GetEnumerator()
		{
			return this.Children.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.Children.GetEnumerator();
		}

		public Parent()
		{
			this.InitializeChildren();
		}
	}
}
