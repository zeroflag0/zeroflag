using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing.Structure
{
	public class ItemNode : Item
	{
		#region Inner
		private List<Item> _Inner = new List<Item>();

		public List<Item> Inner
		{
			get { return _Inner; }
		}
		#endregion Inner

		public override string Value
		{
			get
			{
				StringBuilder builder = new StringBuilder();
				foreach (Item item in this.Inner)
				{
					builder.Append(item);
				}
				return builder.ToString();
			}
			set
			{
			}
		}
	}
}
