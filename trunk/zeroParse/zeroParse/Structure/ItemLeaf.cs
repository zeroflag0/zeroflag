using System;
using System.Collections.Generic;
using System.Text;

namespace zeroflag.Parsing.Structure
{
	public class ItemLeaf : Item
	{
		#region Value

		private string _Value;

		public override string Value
		{
			get { return _Value; }
			set
			{
				if (_Value != value)
				{
					_Value = value;
				}
			}
		}
		#endregion Value
	}
}
