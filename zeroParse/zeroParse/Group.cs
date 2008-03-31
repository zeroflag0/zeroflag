using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public abstract class Group : Rule
	{
		public Group()
		{
		}

		public Group(params Rule[] items)
		{
			this.Items.AddRange(items);
		}

		#region Items
		private List<Rule> _Items;

		/// <summary>
		/// The items in this group.
		/// </summary>
		public List<Rule> Items
		{
			get { return _Items ?? (_Items = this.ItemsCreate); }
		}

		/// <summary>
		/// Creates the default/initial value for Items.
		/// The items in this group.
		/// </summary>
		protected virtual List<Rule> ItemsCreate
		{
			get { return new List<Rule>(); }
		}

		#endregion Items

	}
}
