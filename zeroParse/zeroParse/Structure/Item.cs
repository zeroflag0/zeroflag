using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing.Structure
{
	[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public abstract class Item
	{
		#region Index

		private int _Index = -1;

		public int Index
		{
			get { return _Index; }
			set
			{
				if (_Index != value)
				{
					_Index = value;
				}
			}
		}
		#endregion Index

		public abstract string Value
		{
			get;
			set;
		}

		#region Outer

		private ItemNode _Outer = default(ItemNode);

		public ItemNode Outer
		{
			get { return _Outer; }
			set
			{
				if (_Outer != value)
				{
					_Outer = value;
				}
			}
		}

		#endregion Outer

		#region Name

		private string _Name;

		public string Name
		{
			get { return _Name; }
			set
			{
				if (_Name != value)
				{
					_Name = value;
				}
			}
		}
		#endregion Name
	}
}
