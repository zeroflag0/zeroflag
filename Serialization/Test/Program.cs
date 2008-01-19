using System;
using System.Collections.Generic;
using System.Text;
using zeroflag.Serialization;

namespace Test
{
	public class A
	{
		List<A> _Children = new List<A>();

		public List<A> Children
		{
			get { return _Children; }
			set { _Children = value; }
		}

		public A()
		{
		}

		public A(params A[] children)
		{
			this.Children.AddRange(children);
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			Serializer seri= new XmlSerializer("test.xml");

			A a = new A(new A(), new A(), null);

			seri.Serialize(a);
		}
	}
}
