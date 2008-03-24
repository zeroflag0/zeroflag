using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Test
{
	[Serializable]
	public class TestData : System.Runtime.Serialization.ISerializable
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

		#region ISerializable Members

		public TestData()
		{
		}

		protected TestData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new System.ArgumentNullException("info");
			this.Name = (string)info.GetValue("Name", typeof(string));
			this.Int = (int)info.GetValue("Int", typeof(int));
			this.Hidden = (bool)info.GetValue("Hidden", typeof(bool));
		}

		public virtual void GetObjectData(
		SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new System.ArgumentNullException("info");
			info.AddValue("Name", this.Name);
			info.AddValue("Int", this.Int);
			info.AddValue("Hidden", this.Hidden);
		}

		#endregion
	}
}
