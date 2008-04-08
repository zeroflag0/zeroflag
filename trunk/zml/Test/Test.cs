using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
	public class Test
	{
		string _Name;

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}


		int _Int;

		public int Int
		{
			get { return _Int; }
			set { _Int = value; }
		}

		double _Real;

		public double Real
		{
			get { return _Real; }
			set { _Real = value; }
		}

		List<Test> _Inner = new List<Test>();

		public List<Test> Inner
		{
			get { return _Inner; }
			//set { _Inner = value; }
		}

		//public override string ToString()
		//{
		//    return this.GetType().Name + "[" + this.Name + "," + this.Int + "," + this.Real + "," + this.ToString(this.Inner) + "]";
		//}
		//string ToString(IEnumerable<Test> enu)
		//{
		//    if (enu == null) return null;
		//    StringBuilder b = new StringBuilder("{");
		//    foreach (Test t in enu)
		//    {
		//        b.Append(t).Append(",");
		//    }
		//    b.Append("}");
		//    return b.ToString();
		//}

		public override string ToString()
		{
			return this.ToString(new StringBuilder(), 0).ToString();
		}

		public StringBuilder ToString(StringBuilder b, int depth)
		{
			b.AppendLine().Append(' ', depth).Append(this.GetType().Name).Append("[").Append(this.Name).Append(",").Append(this.Int).Append(",").Append(this.Real).Append(",").Append(this.GetHashCode());
			if (this.Inner != null && depth < 20)
			{
				depth++;
				b.AppendLine().Append(' ', depth).Append("{");
				foreach (Test inner in this.Inner)
				{
					inner.ToString(b, depth);
				}
				depth--;
				b.AppendLine().Append(' ', depth).Append("}");
			}
			b.Append("]");
			return b;
		}

		public Test()
		{
		}

		public Test(string name, int integer, float real)
		{
			this.Name = name;
			this.Int = integer;
			this.Real = real;
		}

		public Test Add(params Test[] inner)
		{
			this.Inner.AddRange(inner);
			return this;
		}
	}

	public class Test2 : Test
	{
		public Test2()
		{
		}

		public Test2(string name, int integer, float real)
			: base(name, integer, real)
		{ }
	}
}
