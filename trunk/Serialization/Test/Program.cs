#region LGPL License
//********************************************************************
//	author:         Thomas "zeroflag" Kraemer
//	author email:   zeroflag@zeroflag.de
//	
//	Copyright (C) 2006-2008  Thomas "zeroflag" Kraemer
//	
//	license:	(LGPL)
//	
//		This library is free software; you can redistribute it and/or
//		modify it under the terms of the GNU Lesser General Public
//		License as published by the Free Software Foundation; either
//		version 2.1 of the License, or (at your option) any later version.
//
//		This library is distributed in the hope that it will be useful,
//		but WITHOUT ANY WARRANTY; without even the implied warranty of
//		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//		Lesser General Public License for more details.
//
//		You should have received a copy of the GNU Lesser General Public
//		License along with this library; if not, write to the Free Software
//		Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//
//		http://www.gnu.org/licenses/lgpl.html#TOC1
//
//*********************************************************************
#endregion LGPL License

//#define TEST1 // class A
#define TEST2 // dictionary
//#define TEST3 // winforms <-- doesn't work and it's not because my serializer is too stupid...

using System;
using System.Collections.Generic;
using System.Text;
using zeroflag.Serialization;

namespace Test
{
#if TEST1 || TEST2
	public class A
	{
		A _Parent;

		public A Parent
		{
			get { return _Parent; }
			set { _Parent = value; }
		}

		List<A> _Children = new List<A>();

		public List<A> Children
		{
			get { return _Children; }
			set { _Children = value; }
		}

		string _Name = null;

		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}

		public A()
		{
		}

		public A(string name)
		{
			this.Name = name;
		}

		public A(string name, params A[] children)
			: this(name)
		{
			foreach (A child in children)
			{
				this.Children.Add(child);
				if (child != null)
					child.Parent = this;
			}
			//this.Children.AddRange(children);
		}

		public override string ToString()
		{
			string value = this.Name ?? "<null>";
			value += "[";
			if (this.Parent != null)
				value += this.Parent.Name;
			value += "]";
			value += "{";
			if (this.Children.Count > 0)
			{
				foreach (A a in this.Children)
					value += a + ", ";
			}
			value = value.TrimEnd(',', ' ') + "}";
			return value;
		}

	}
#endif
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
#if TEST1
				Serializer seri = new XmlSerializer("test.xml");

				A a = new A("root", new A("foo"), new A("bar"), new A(null), null);

				//zeroflag.Serialization.Descriptors.Descriptor desc = zeroflag.Serialization.Descriptors.Descriptor.DoParse(a);

				//Console.WriteLine(desc);
				//seri.Serialize(a);
				//seri.Serialize(desc);

				A b = null;
				//b = (A)desc.Generate();
				b = seri.Deserialize<A>();

				Console.Write("a = " + a);
				Console.WriteLine(" b = " + b);
				//b.Children[2] = new A("new");
				//Console.WriteLine("Modified b...");
				//Console.Write("a = " + a);
				//Console.WriteLine(" b = " + b);
#endif
#if TEST2
				//TestDict<string, int>("foo", 1, "bar", 2, "bla", -1);

				//TestDict<string, double>("foo", 1, "bar", 2, "bla", -1);

				A foo = new A("foo"), bar = new A("bar", foo), bla = new A("bla", foo, bar);
				Console.WriteLine("foo = " + foo);
				Console.WriteLine("bar = " + bar);
				Console.WriteLine("bla = " + bla);
				TestDict<string, A>("foo", foo, "bar", bar, "bla", bla);
#endif
#if TEST3
				System.Windows.Forms.Application.Run(new TestForm());

				TestForm restore = new zeroflag.Serialization.XmlSerializer("test_form.xml").Deserialize<TestForm>();
				System.Windows.Forms.Application.Run(restore);
#endif
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc);
			}
			finally { }
		}

#if TEST2
		static void TestDict<T1, T2>(T1 p11, T2 p12, T1 p21, T2 p22, T1 p31, T2 p32)
		{
			Serializer seri2 = new XmlSerializer("test_dict_" + typeof(T1).Name + "_" + typeof(T2).Name + ".xml");
			Dictionary<T1, T2> dict = new Dictionary<T1, T2>();
			dict.Add((T1)p11, (T2)p12);
			dict.Add((T1)p21, (T2)p22);
			dict.Add((T1)p31, (T2)p32);
			seri2.Serialize(dict);

			Dictionary<T1, T2> result = seri2.Deserialize<Dictionary<T1, T2>>();
			Console.WriteLine("<TestDictResult>");
			foreach (KeyValuePair<T1, T2> pair in result)
			{
				Console.WriteLine("\t" + pair.Key + " => " + pair.Value);
			}
			Console.WriteLine("</TestDictResult>");
		}
#endif

	}
}
