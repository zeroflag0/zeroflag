using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
	public class TestData
	{
		#region Name

		private string _Name = default(string);

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

		#region Int

		private int _Int = default(int);

		public int Int
		{
			get { return _Int; }
			set
			{
				if (_Int != value)
				{
					_Int = value;
				}
			}
		}
		#endregion Int

		#region Hidden

		private bool _Hidden = default(bool);

		[System.ComponentModel.Browsable(false)]
		public bool Hidden
		{
			get { return _Hidden; }
			set
			{
				if (_Hidden != value)
				{
					_Hidden = value;
				}
			}
		}
		#endregion Hidden
	}
}
