using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Test
{
	[Serializable]
	public class TestData //: System.Runtime.Serialization.ISerializable
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

		//public TestData()
		//{
		//}

		//protected TestData(SerializationInfo info, StreamingContext context)
		//{
		//    if (info == null)
		//        throw new System.ArgumentNullException("info");
		//    this.Name = (string)info.GetValue("Name", typeof(string));
		//    this.Int = (int)info.GetValue("Int", typeof(int));
		//    this.Hidden = (bool)info.GetValue("Hidden", typeof(bool));
		//}

		//public virtual void GetObjectData(
		//SerializationInfo info, StreamingContext context)
		//{
		//    if (info == null)
		//        throw new System.ArgumentNullException("info");
		//    info.AddValue("Name", this.Name);
		//    info.AddValue("Int", this.Int);
		//    info.AddValue("Hidden", this.Hidden);
		//}

		#endregion

		double _Real;

		public double Real
		{
			get { return _Real; }
			set { _Real = value; }
		}

		List<TestData> _Inner = new List<TestData>();

		[System.ComponentModel.Browsable(false)]
		public List<TestData> Inner
		{
			get { return _Inner; }
			//set { _Inner = value; }
		}

		public override string ToString()
		{
			return this.ToString(new StringBuilder(), new List<TestData>(), 0).ToString();
		}

		public StringBuilder ToString(StringBuilder b, List<TestData> done, int depth)
		{
			b.AppendLine().Append(' ', depth).Append(this.GetType().Name).Append("[").Append(this.Name).Append(",").Append(this.Int).Append(",").Append(this.Real).Append(",").Append(this.GetHashCode());
			if (this.Inner != null)
			{
				if (depth > 10)
				{
					b.Append(",<...>");
				}
				else if (done.Contains(this))
				{
					b.Append(",<link>");
				}
				else
				{
					done.Add(this);
					depth++;
					b.AppendLine().Append(' ', depth).Append("{");
					foreach (TestData inner in this.Inner)
					{
						inner.ToString(b, done, depth);
					}
					depth--;
					b.AppendLine().Append(' ', depth).Append("}");
				}
			}
			b.Append("]");
			return b;
		}

		public TestData()
		{
		}

		public TestData(string name, int integer, float real)
		{
			this.Name = name;
			this.Int = integer;
			this.Real = real;
		}

		public TestData Add(params TestData[] inner)
		{
			this.Inner.AddRange(inner);
			return this;
		}
	}

	public class TestData2 : TestData
	{
		public TestData2()
		{
		}

		public TestData2(string name, int integer, float real)
			: base(name, integer, real)
		{ }
	}
}
