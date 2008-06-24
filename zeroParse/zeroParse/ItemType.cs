using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing
{
	public abstract class ItemType
	{

		#region Name

		private string _Name;

		public string Name
		{
			get { return _Name; }
			set
			{
				if ( _Name != value )
				{
					_Name = value;
				}
			}
		}
		#endregion Name

		#region Value

		private string _Value;

		public string Value
		{
			get { return _Value; }
			set
			{
				if ( _Value != value )
				{
					_Value = value;
				}
			}
		}
		#endregion Value


		#region Index

		private int? _Index;

		public int? Index
		{
			get { return _Index; }
			set
			{
				if ( _Index != value )
				{
					_Index = value;
				}
			}
		}
		#endregion Index

		
		#region Snippet

		private string _Snippet;

		public string Snippet
		{
			get { return _Snippet; }
			set
			{
				if ( _Snippet != value )
				{
					_Snippet = value;
				}
			}
		}
		#endregion Snippet


		#region Outer

		private ItemType _Outer;

		public ItemType Outer
		{
			get { return _Outer; }
			set
			{
				if ( _Outer != value )
				{
					_Outer = value;
				}
			}
		}
		#endregion Outer

		#region Inner
		private List<ItemType> _Inner;

		/// <summary>
		/// Inner items.
		/// </summary>
		public List<ItemType> Inner
		{
			get { return _Inner ?? ( _Inner = this.InnerCreate ); }
		}

		/// <summary>
		/// Creates the default/initial value for Inner.
		/// Inner items.
		/// </summary>
		protected virtual List<ItemType> InnerCreate
		{
			get { return new List<ItemType>(); }
		}

		#endregion Inner

	}
}
